using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Attendance.Queries.GetAttendanceSummary;

public class GetAttendanceSummaryQueryHandler : IRequestHandler<GetAttendanceSummaryQuery, ErrorOr<AttendanceSummaryResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetAttendanceSummaryQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<AttendanceSummaryResult>> Handle(GetAttendanceSummaryQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

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

        // Enrolled students
        var enrolledStudents = await _db.Enrollments
            .AsNoTracking()
            .Where(e => e.SectionId == request.SectionId && e.Status == EnrollmentStatus.Active)
            .Include(e => e.Student)
            .Select(e => new { e.Student.Id, e.Student.FirstName, e.Student.LastName, e.Student.StudentCode })
            .ToListAsync(cancellationToken);

        // All records for this section/period
        var allRecords = await _db.AttendanceRecords
            .AsNoTracking()
            .Where(a => a.SectionId == request.SectionId && a.AcademicPeriodId == request.AcademicPeriodId)
            .ToListAsync(cancellationToken);

        var totalStudents = enrolledStudents.Count;
        var schoolDays = allRecords.Select(r => r.Date).Distinct().Count();

        // Per-student absence stats for risk detection
        var studentStats = enrolledStudents.Select(student =>
        {
            var studentRecords = allRecords.Where(r => r.StudentId == student.Id).ToList();
            var absentDays = studentRecords.Count(r => r.Status == AttendanceStatus.Absent);
            var totalDays = studentRecords.Count;
            var absenceRate = totalDays > 0 ? Math.Round((decimal)absentDays / totalDays * 100, 1) : 0;

            var riskLevel = absenceRate switch
            {
                >= 30 => "Critical",
                >= 20 => "High",
                >= 10 => "Medium",
                _ => "Low"
            };

            return new StudentRiskSummary(
                StudentId: student.Id,
                FullName: $"{student.FirstName} {student.LastName}",
                StudentCode: student.StudentCode,
                AbsentDays: absentDays,
                AbsenceRate: absenceRate,
                RiskLevel: riskLevel);
        }).ToList();

        var atRiskStudents = studentStats
            .Where(s => s.RiskLevel is "High" or "Critical")
            .OrderByDescending(s => s.AbsenceRate)
            .ToList();

        // Daily stats
        var dailyStats = allRecords
            .GroupBy(r => r.Date)
            .OrderBy(g => g.Key)
            .Select(g =>
            {
                var present = g.Count(r => r.Status is AttendanceStatus.Present or AttendanceStatus.Late or AttendanceStatus.HalfDay or AttendanceStatus.Remote);
                var rate = totalStudents > 0 ? Math.Round((decimal)present / totalStudents * 100, 1) : 0;

                return new DailyStat(
                    Date: g.Key,
                    Present: present,
                    Absent: g.Count(r => r.Status == AttendanceStatus.Absent),
                    Late: g.Count(r => r.Status == AttendanceStatus.Late),
                    Excused: g.Count(r => r.Status == AttendanceStatus.Excused),
                    AttendanceRate: rate);
            })
            .ToList();

        var avgAttendanceRate = dailyStats.Count > 0
            ? Math.Round(dailyStats.Average(d => d.AttendanceRate), 1)
            : 0;

        return new AttendanceSummaryResult(
            SectionId: section.Id,
            SectionName: section.Name,
            GradeLevel: section.GradeLevel.Name,
            PeriodName: period.Name,
            TotalStudents: totalStudents,
            TotalSchoolDays: schoolDays,
            AverageAttendanceRate: avgAttendanceRate,
            AtRiskStudents: atRiskStudents,
            DailyStats: dailyStats);
    }
}
