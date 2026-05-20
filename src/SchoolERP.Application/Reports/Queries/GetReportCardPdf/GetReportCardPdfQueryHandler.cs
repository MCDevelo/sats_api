using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Models;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Reports.Queries.GetReportCardPdf;

public class GetReportCardPdfQueryHandler
    : IRequestHandler<GetReportCardPdfQuery, ErrorOr<ReportResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IReportGeneratorService _reportGenerator;

    public GetReportCardPdfQueryHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        IReportGeneratorService reportGenerator)
    {
        _db = db;
        _currentUser = currentUser;
        _reportGenerator = reportGenerator;
    }

    public async Task<ErrorOr<ReportResult>> Handle(
        GetReportCardPdfQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var student = await _db.Students
            .Include(s => s.School)
            .FirstOrDefaultAsync(s =>
                s.Id == request.StudentId &&
                s.TenantId == tenantId, cancellationToken);

        if (student is null)
            return Error.NotFound(description: "Estudiante no encontrado.");

        var period = await _db.AcademicPeriods
            .Include(p => p.AcademicYear)
            .FirstOrDefaultAsync(p => p.Id == request.AcademicPeriodId, cancellationToken);

        if (period is null)
            return Error.NotFound(description: "Período académico no encontrado.");

        // Active enrollment → section & grade level
        var enrollment = await _db.Enrollments
            .Include(e => e.Section).ThenInclude(s => s.GradeLevel)
            .FirstOrDefaultAsync(e =>
                e.StudentId == request.StudentId &&
                e.Section.AcademicYearId == period.AcademicYearId &&
                e.Status == EnrollmentStatus.Active, cancellationToken);

        if (enrollment is null)
            return Error.NotFound(description: "El estudiante no tiene matrícula activa en ese año académico.");

        // Grade entries for this student and period
        var entries = await _db.GradeEntries
            .Include(ge => ge.Subject)
            .Include(ge => ge.EvaluationPlan)
            .Where(ge =>
                ge.StudentId == request.StudentId &&
                ge.AcademicPeriodId == request.AcademicPeriodId &&
                ge.TenantId == tenantId)
            .ToListAsync(cancellationToken);

        // Build per-subject rows
        var subjectGroups = entries
            .GroupBy(ge => ge.Subject)
            .OrderBy(g => g.Key.Name);

        var subjectRows = subjectGroups.Select(g =>
        {
            var evalItems = g.Select(ge => new EvalItemRow(
                ge.EvaluationPlan.Name,
                ge.EvaluationPlan.Weight,
                ge.Score)).ToList();

            var totalWeight = evalItems.Sum(e => e.Weight);
            var weightedAvg = totalWeight > 0
                ? evalItems.Sum(e => e.Score * e.Weight) / totalWeight
                : 0m;

            weightedAvg = Math.Round(weightedAvg, 2);
            var isPassing = weightedAvg >= 60;
            var letter = weightedAvg switch
            {
                >= 90 => "A",
                >= 80 => "B",
                >= 70 => "C",
                >= 60 => "D",
                _     => "F"
            };

            return new SubjectGradeRow(
                g.Key.Name,
                evalItems,
                weightedAvg,
                letter,
                isPassing);
        }).ToList();

        // General average (simple mean of subject averages)
        var generalAverage = subjectRows.Count > 0
            ? Math.Round(subjectRows.Average(s => s.WeightedAverage), 2)
            : 0m;

        var isPromoted = subjectRows.All(s => s.IsPassing);

        // Rank: compare against all active students in the same section
        var sectionStudentIds = await _db.Enrollments
            .Where(e =>
                e.SectionId == enrollment.SectionId &&
                e.Status == EnrollmentStatus.Active)
            .Select(e => e.StudentId)
            .ToListAsync(cancellationToken);

        var allSectionEntries = await _db.GradeEntries
            .Include(ge => ge.EvaluationPlan)
            .Where(ge =>
                ge.AcademicPeriodId == request.AcademicPeriodId &&
                ge.TenantId == tenantId &&
                sectionStudentIds.Contains(ge.StudentId))
            .ToListAsync(cancellationToken);

        var studentAverages = allSectionEntries
            .GroupBy(ge => ge.StudentId)
            .Select(g =>
            {
                var subjectAvgs = g.GroupBy(x => x.SubjectId)
                    .Select(sg =>
                    {
                        var totalW = sg.Sum(x => x.EvaluationPlan.Weight);
                        return totalW > 0 ? sg.Sum(x => x.Score * x.EvaluationPlan.Weight) / totalW : 0m;
                    }).ToList();
                return new { StudentId = g.Key, Avg = subjectAvgs.Count > 0 ? subjectAvgs.Average() : 0m };
            }).ToList();

        var totalStudents = studentAverages.Count;
        var rank = studentAverages.Count(s => s.Avg > generalAverage) + 1;

        // Attendance summary
        var attendanceRecords = await _db.AttendanceRecords
            .Where(r =>
                r.StudentId == request.StudentId &&
                r.AcademicPeriodId == request.AcademicPeriodId &&
                r.TenantId == tenantId)
            .ToListAsync(cancellationToken);

        AttendanceSummaryData? attendance = null;
        if (attendanceRecords.Count > 0)
        {
            var present = attendanceRecords.Count(r => r.Status == AttendanceStatus.Present);
            var late    = attendanceRecords.Count(r => r.Status == AttendanceStatus.Late);
            var absent  = attendanceRecords.Count(r => r.Status == AttendanceStatus.Absent);
            var excused = attendanceRecords.Count(r => r.Status == AttendanceStatus.Excused);
            var total   = attendanceRecords.Count;
            attendance = new AttendanceSummaryData(
                present, absent, late, excused,
                Math.Round((decimal)(present + late) / total * 100, 1));
        }

        var data = new ReportCardData(
            SchoolName: student.School.Name,
            StudentFullName: student.FullName,
            StudentCode: student.StudentCode,
            GradeLevel: enrollment.Section.GradeLevel.Name,
            SectionName: enrollment.Section.Name,
            PeriodName: period.Name,
            AcademicYear: period.AcademicYear.Name,
            GeneratedAt: DateTime.UtcNow,
            Subjects: subjectRows,
            GeneralAverage: generalAverage,
            IsPromoted: isPromoted,
            Rank: totalStudents > 0 ? rank : null,
            TotalStudents: totalStudents,
            Attendance: attendance);

        var pdf = _reportGenerator.GenerateReportCard(data);
        var safeName = student.FullName.Replace(" ", "_");
        var fileName = $"boletin_{safeName}_{period.Name.Replace(" ", "_")}_{DateTime.UtcNow:yyyyMMdd}.pdf";

        return new ReportResult(pdf, fileName);
    }
}
