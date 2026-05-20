using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Grades.Queries.GetGradeBook;

public class GetGradeBookQueryHandler : IRequestHandler<GetGradeBookQuery, ErrorOr<GradeBookResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetGradeBookQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<GradeBookResult>> Handle(GetGradeBookQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var subject = await _db.Subjects
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.SubjectId && s.TenantId == tenantId && s.IsActive, cancellationToken);

        if (subject is null)
            return Error.NotFound(description: "Materia no encontrada.");

        var section = await _db.Sections
            .AsNoTracking()
            .Include(s => s.GradeLevel)
            .FirstOrDefaultAsync(s => s.Id == request.SectionId && s.TenantId == tenantId, cancellationToken);

        if (section is null)
            return Error.NotFound(description: "Sección no encontrada.");

        var period = await _db.AcademicPeriods
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.AcademicPeriodId, cancellationToken);

        if (period is null)
            return Error.NotFound(description: "Período académico no encontrado.");

        // Evaluation plans for this subject/period
        var plans = await _db.EvaluationPlans
            .AsNoTracking()
            .Where(ep => ep.SubjectId == request.SubjectId && ep.AcademicPeriodId == request.AcademicPeriodId)
            .OrderBy(ep => ep.CreatedAt)
            .ToListAsync(cancellationToken);

        // Enrolled students in this section
        var enrolledStudents = await _db.Enrollments
            .AsNoTracking()
            .Where(e => e.SectionId == request.SectionId && e.Status == EnrollmentStatus.Active)
            .Include(e => e.Student)
            .OrderBy(e => e.Student.LastName).ThenBy(e => e.Student.FirstName)
            .Select(e => new { e.Student.Id, e.Student.FirstName, e.Student.LastName, e.Student.SecondLastName, e.Student.StudentCode })
            .ToListAsync(cancellationToken);

        // All grade entries for these students/plans
        var planIds = plans.Select(p => p.Id).ToList();
        var studentIds = enrolledStudents.Select(s => s.Id).ToList();

        var allGradeEntries = await _db.GradeEntries
            .AsNoTracking()
            .Where(ge =>
                ge.SubjectId == request.SubjectId &&
                ge.AcademicPeriodId == request.AcademicPeriodId &&
                planIds.Contains(ge.EvaluationPlanId) &&
                studentIds.Contains(ge.StudentId))
            .ToListAsync(cancellationToken);

        var gradeMap = allGradeEntries
            .GroupBy(ge => ge.StudentId)
            .ToDictionary(g => g.Key, g => g.ToDictionary(ge => ge.EvaluationPlanId));

        var totalWeight = plans.Sum(p => p.Weight);

        var evaluationColumns = plans.Select(p => new EvaluationColumn(
            PlanId: p.Id,
            Name: p.Name,
            Weight: p.Weight,
            Description: p.Description,
            DueDate: p.DueDate,
            IsPublished: p.IsPublished)).ToList();

        var studentRows = enrolledStudents.Select(student =>
        {
            gradeMap.TryGetValue(student.Id, out var studentGrades);
            studentGrades ??= new Dictionary<Guid, Domain.Entities.GradeEntry>();

            var cells = plans.Select(plan =>
            {
                studentGrades.TryGetValue(plan.Id, out var entry);
                return new GradeCell(
                    PlanId: plan.Id,
                    GradeEntryId: entry?.Id,
                    Score: entry?.Score,
                    LetterGrade: entry?.GetLetterGrade(),
                    Comments: entry?.Comments,
                    IsPublished: entry?.IsPublished ?? false);
            }).ToList();

            // Weighted average: only plans that have grades
            decimal? weightedAvg = null;
            var gradedPlans = plans.Where(p => studentGrades.ContainsKey(p.Id)).ToList();

            if (gradedPlans.Count > 0)
            {
                var usedWeight = gradedPlans.Sum(p => p.Weight);
                if (usedWeight > 0)
                {
                    weightedAvg = gradedPlans.Sum(p => studentGrades[p.Id].Score * (p.Weight / usedWeight));
                    weightedAvg = Math.Round(weightedAvg.Value, 2);
                }
            }

            var isPassing = weightedAvg.HasValue && weightedAvg.Value >= 60;
            var letterGrade = weightedAvg.HasValue ? GetLetterGrade(weightedAvg.Value) : null;

            return new StudentGradeRow(
                StudentId: student.Id,
                FullName: string.IsNullOrEmpty(student.SecondLastName) ? $"{student.FirstName} {student.LastName}" : $"{student.FirstName} {student.LastName} {student.SecondLastName}",
                StudentCode: student.StudentCode,
                Grades: cells,
                WeightedAverage: weightedAvg,
                FinalLetterGrade: letterGrade,
                IsPassing: isPassing);
        }).ToList();

        return new GradeBookResult(
            SubjectId: subject.Id,
            SubjectName: subject.Name,
            SectionId: section.Id,
            SectionName: section.Name,
            GradeLevel: section.GradeLevel.Name,
            PeriodName: period.Name,
            GradesPublished: period.GradesPublished,
            TotalWeight: totalWeight,
            Evaluations: evaluationColumns,
            Students: studentRows);
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
