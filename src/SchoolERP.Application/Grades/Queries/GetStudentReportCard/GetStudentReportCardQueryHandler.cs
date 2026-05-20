using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Grades.Queries.GetStudentReportCard;

public class GetStudentReportCardQueryHandler : IRequestHandler<GetStudentReportCardQuery, ErrorOr<ReportCardResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetStudentReportCardQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<ReportCardResult>> Handle(GetStudentReportCardQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var student = await _db.Students
            .AsNoTracking()
            .Include(s => s.School)
            .FirstOrDefaultAsync(s => s.Id == request.StudentId && s.TenantId == tenantId, cancellationToken);

        if (student is null)
            return Error.NotFound(description: "Estudiante no encontrado.");

        var period = await _db.AcademicPeriods
            .AsNoTracking()
            .Include(p => p.AcademicYear)
            .FirstOrDefaultAsync(p => p.Id == request.AcademicPeriodId, cancellationToken);

        if (period is null)
            return Error.NotFound(description: "Período académico no encontrado.");

        // Active enrollment for this academic year
        var enrollment = await _db.Enrollments
            .AsNoTracking()
            .Include(e => e.Section)
                .ThenInclude(s => s.GradeLevel)
            .FirstOrDefaultAsync(e =>
                e.StudentId == request.StudentId &&
                e.AcademicYearId == period.AcademicYear.Id &&
                e.Status == EnrollmentStatus.Active, cancellationToken);

        if (enrollment is null)
            return Error.NotFound(description: "El estudiante no tiene matrícula activa en este año académico.");

        // Subjects for the student's grade level
        var subjects = await _db.Subjects
            .AsNoTracking()
            .Where(s => s.GradeLevelId == enrollment.Section.GradeLevelId && s.TenantId == tenantId && s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);

        // All evaluation plans for this period
        var subjectIds = subjects.Select(s => s.Id).ToList();
        var plans = await _db.EvaluationPlans
            .AsNoTracking()
            .Where(ep => subjectIds.Contains(ep.SubjectId) && ep.AcademicPeriodId == request.AcademicPeriodId)
            .OrderBy(ep => ep.CreatedAt)
            .ToListAsync(cancellationToken);

        // Grade entries for this student/period
        var gradeEntries = await _db.GradeEntries
            .AsNoTracking()
            .Where(ge => ge.StudentId == request.StudentId && ge.AcademicPeriodId == request.AcademicPeriodId)
            .ToListAsync(cancellationToken);

        var planMap = plans.GroupBy(p => p.SubjectId).ToDictionary(g => g.Key, g => g.ToList());
        var gradeMap = gradeEntries.ToDictionary(ge => ge.EvaluationPlanId);

        var subjectGrades = subjects.Select(subject =>
        {
            planMap.TryGetValue(subject.Id, out var subjectPlans);
            subjectPlans ??= new List<Domain.Entities.EvaluationPlan>();

            var evaluations = subjectPlans.Select(plan =>
            {
                gradeMap.TryGetValue(plan.Id, out var entry);
                return new EvaluationGrade(
                    EvaluationName: plan.Name,
                    Weight: plan.Weight,
                    Score: entry?.Score,
                    LetterGrade: entry?.GetLetterGrade());
            }).ToList();

            // Weighted final grade
            decimal? finalGrade = null;
            var scoredPlans = subjectPlans.Where(p => gradeMap.ContainsKey(p.Id)).ToList();
            if (scoredPlans.Count > 0)
            {
                var usedWeight = scoredPlans.Sum(p => p.Weight);
                if (usedWeight > 0)
                    finalGrade = Math.Round(scoredPlans.Sum(p => gradeMap[p.Id].Score * (p.Weight / usedWeight)), 2);
            }

            var isPassing = finalGrade.HasValue && finalGrade.Value >= 60;

            return new SubjectGrade(
                SubjectId: subject.Id,
                SubjectName: subject.Name,
                WeeklyHours: subject.WeeklyHours,
                Evaluations: evaluations,
                FinalGrade: finalGrade,
                LetterGrade: finalGrade.HasValue ? GetLetterGrade(finalGrade.Value) : null,
                IsPassing: isPassing);
        }).ToList();

        // General average across all subjects that have at least one grade
        var gradedSubjects = subjectGrades.Where(sg => sg.FinalGrade.HasValue).ToList();
        decimal? generalAverage = gradedSubjects.Count > 0
            ? Math.Round(gradedSubjects.Average(sg => sg.FinalGrade!.Value), 2)
            : null;

        // Rank: position among classmates (requires separate query)
        var rank = await CalculateRankAsync(request.StudentId, enrollment.SectionId, request.AcademicPeriodId, generalAverage, cancellationToken);

        var isPromoted = generalAverage.HasValue && generalAverage.Value >= 60 &&
                         subjectGrades.Count(sg => !sg.IsPassing && sg.FinalGrade.HasValue) == 0;

        return new ReportCardResult(
            StudentId: student.Id,
            StudentFullName: student.FullName,
            StudentCode: student.StudentCode,
            SchoolName: student.School.Name,
            GradeLevel: enrollment.Section.GradeLevel.Name,
            Section: enrollment.Section.Name,
            AcademicYear: period.AcademicYear.Name,
            Period: period.Name,
            IsPublished: period.GradesPublished,
            Subjects: subjectGrades,
            GeneralAverage: generalAverage,
            GeneralLetterGrade: generalAverage.HasValue ? GetLetterGrade(generalAverage.Value) : null,
            IsPromoted: isPromoted,
            Rank: rank);
    }

    private async Task<int> CalculateRankAsync(
        Guid studentId,
        Guid sectionId,
        Guid periodId,
        decimal? studentAverage,
        CancellationToken cancellationToken)
    {
        if (!studentAverage.HasValue) return 0;

        // Get averages for all classmates in the same section/period
        var classmates = await _db.Enrollments
            .AsNoTracking()
            .Where(e => e.SectionId == sectionId && e.Status == EnrollmentStatus.Active)
            .Select(e => e.StudentId)
            .ToListAsync(cancellationToken);

        var classmateAverages = await _db.GradeEntries
            .AsNoTracking()
            .Where(ge => ge.AcademicPeriodId == periodId && classmates.Contains(ge.StudentId))
            .GroupBy(ge => ge.StudentId)
            .Select(g => new { StudentId = g.Key, Average = (decimal)g.Average(ge => (double)ge.Score) })
            .ToListAsync(cancellationToken);

        var rank = classmateAverages.Count(c => c.Average > studentAverage!.Value) + 1;
        return rank;
    }

    private static string GetLetterGrade(decimal score) => score switch
    {
        >= 90 => "A",
        >= 80 => "B",
        >= 70 => "C",
        >= 60 => "D",
        _ => "F"
    };
}
