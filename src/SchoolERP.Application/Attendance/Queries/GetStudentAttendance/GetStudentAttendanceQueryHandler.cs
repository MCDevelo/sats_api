using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Attendance.Queries.GetStudentAttendance;

public class GetStudentAttendanceQueryHandler : IRequestHandler<GetStudentAttendanceQuery, ErrorOr<StudentAttendanceResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetStudentAttendanceQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<StudentAttendanceResult>> Handle(GetStudentAttendanceQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var student = await _db.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.StudentId && s.TenantId == tenantId, cancellationToken);

        if (student is null)
            return Error.NotFound(description: "Estudiante no encontrado.");

        var period = await _db.AcademicPeriods
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.AcademicPeriodId, cancellationToken);

        if (period is null)
            return Error.NotFound(description: "Período académico no encontrado.");

        var records = await _db.AttendanceRecords
            .AsNoTracking()
            .Where(a => a.StudentId == request.StudentId && a.AcademicPeriodId == request.AcademicPeriodId)
            .OrderBy(a => a.Date)
            .ToListAsync(cancellationToken);

        var totalDays = records.Count;
        var present = records.Count(r => r.Status == AttendanceStatus.Present);
        var absent = records.Count(r => r.Status == AttendanceStatus.Absent);
        var late = records.Count(r => r.Status == AttendanceStatus.Late);
        var excused = records.Count(r => r.Status == AttendanceStatus.Excused);
        var halfDay = records.Count(r => r.Status == AttendanceStatus.HalfDay);
        var remote = records.Count(r => r.Status == AttendanceStatus.Remote);

        var effectivePresent = present + late + halfDay + remote;
        var attendanceRate = totalDays > 0 ? Math.Round((decimal)effectivePresent / totalDays * 100, 1) : 0;
        var absenceRate = totalDays > 0 ? Math.Round((decimal)absent / totalDays * 100, 1) : 0;

        var (riskLevel, riskDescription) = CalculateRisk(absenceRate, absent);

        var summary = new AttendanceSummary(
            TotalSchoolDays: totalDays,
            Present: present,
            Absent: absent,
            Late: late,
            Excused: excused,
            HalfDay: halfDay,
            Remote: remote,
            AttendanceRate: attendanceRate,
            AbsenceRate: absenceRate,
            RiskLevel: riskLevel,
            RiskDescription: riskDescription);

        var dailyRecords = records.Select(r => new DailyAttendanceRecord(
            RecordId: r.Id,
            Date: r.Date,
            Status: r.Status.ToString(),
            Notes: r.Notes,
            ArrivalTime: r.ArrivalTime)).ToList();

        return new StudentAttendanceResult(
            StudentId: student.Id,
            StudentFullName: student.FullName,
            StudentCode: student.StudentCode,
            AcademicPeriodId: period.Id,
            PeriodName: period.Name,
            Summary: summary,
            Records: dailyRecords);
    }

    private static (RiskLevel level, string description) CalculateRisk(decimal absenceRate, int absentDays)
    {
        return absenceRate switch
        {
            >= 30 => (RiskLevel.Critical, $"Riesgo crítico: {absentDays} ausencias ({absenceRate}%). Intervención inmediata requerida."),
            >= 20 => (RiskLevel.High, $"Riesgo alto: {absentDays} ausencias ({absenceRate}%). Se recomienda contactar al tutor."),
            >= 10 => (RiskLevel.Medium, $"Riesgo medio: {absentDays} ausencias ({absenceRate}%). Monitoreo continuo."),
            _ => (RiskLevel.Low, $"Asistencia satisfactoria: {absenceRate}% de ausencias.")
        };
    }
}
