using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Models;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Reports.Queries.GetAttendanceReport;

public class GetAttendanceReportQueryHandler
    : IRequestHandler<GetAttendanceReportQuery, ErrorOr<ReportResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IReportGeneratorService _reportGenerator;

    public GetAttendanceReportQueryHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        IReportGeneratorService reportGenerator)
    {
        _db = db;
        _currentUser = currentUser;
        _reportGenerator = reportGenerator;
    }

    public async Task<ErrorOr<ReportResult>> Handle(
        GetAttendanceReportQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var section = await _db.Sections
            .Include(s => s.GradeLevel)
            .Include(s => s.School)
            .FirstOrDefaultAsync(s =>
                s.Id == request.SectionId &&
                s.TenantId == tenantId, cancellationToken);

        if (section is null)
            return Error.NotFound(description: "Sección no encontrada.");

        var period = await _db.AcademicPeriods
            .FirstOrDefaultAsync(p => p.Id == request.AcademicPeriodId, cancellationToken);

        if (period is null)
            return Error.NotFound(description: "Período académico no encontrado.");

        // Active enrollments
        var enrollments = await _db.Enrollments
            .Include(e => e.Student)
            .Where(e =>
                e.SectionId == request.SectionId &&
                e.Status == EnrollmentStatus.Active)
            .OrderBy(e => e.Student.LastName)
                .ThenBy(e => e.Student.FirstName)
            .ToListAsync(cancellationToken);

        // All attendance records for the section+period
        var records = await _db.AttendanceRecords
            .Where(r =>
                r.SectionId == request.SectionId &&
                r.AcademicPeriodId == request.AcademicPeriodId)
            .ToListAsync(cancellationToken);

        var totalDays = records.Select(r => r.Date).Distinct().Count();

        var rows = enrollments.Select((e, idx) =>
        {
            var studentRecords = records.Where(r => r.StudentId == e.StudentId).ToList();
            var present = studentRecords.Count(r => r.Status == AttendanceStatus.Present);
            var late    = studentRecords.Count(r => r.Status == AttendanceStatus.Late);
            var absent  = studentRecords.Count(r => r.Status == AttendanceStatus.Absent);
            var excused = studentRecords.Count(r => r.Status == AttendanceStatus.Excused);
            var total   = present + late + absent + excused;
            var rate    = total > 0 ? (decimal)(present + late) / total * 100 : 0m;

            return new StudentAttendanceRow(
                Index: idx + 1,
                FullName: e.Student.FullName,
                StudentCode: e.Student.StudentCode,
                Present: present,
                Absent: absent,
                Late: late,
                Excused: excused,
                TotalDays: total > 0 ? total : totalDays,
                AttendanceRate: Math.Round(rate, 1));
        }).ToList();

        var data = new AttendanceReportData(
            SchoolName: section.School.Name,
            SectionName: section.Name,
            GradeLevel: section.GradeLevel.Name,
            PeriodName: period.Name,
            StartDate: period.StartDate,
            EndDate: period.EndDate,
            GeneratedAt: DateTime.UtcNow,
            Rows: rows);

        var pdf = _reportGenerator.GenerateAttendanceReport(data);
        var fileName = $"asistencia_{section.Name}_{period.Name.Replace(" ", "_")}_{DateTime.UtcNow:yyyyMMdd}.pdf";

        return new ReportResult(pdf, fileName);
    }
}
