using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Attendance.Queries.GetSectionAttendance;

public class GetSectionAttendanceQueryHandler : IRequestHandler<GetSectionAttendanceQuery, ErrorOr<SectionAttendanceResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetSectionAttendanceQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<SectionAttendanceResult>> Handle(GetSectionAttendanceQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var section = await _db.Sections
            .AsNoTracking()
            .Include(s => s.GradeLevel)
            .FirstOrDefaultAsync(s => s.Id == request.SectionId && s.TenantId == tenantId, cancellationToken);

        if (section is null)
            return Error.NotFound(description: "Sección no encontrada.");

        // All actively enrolled students
        var enrolledStudents = await _db.Enrollments
            .AsNoTracking()
            .Where(e => e.SectionId == request.SectionId && e.Status == EnrollmentStatus.Active)
            .Include(e => e.Student)
            .Select(e => new { e.Student.Id, e.Student.FirstName, e.Student.LastName, e.Student.SecondLastName, e.Student.StudentCode })
            .ToListAsync(cancellationToken);

        // Existing attendance records for this date
        var records = await _db.AttendanceRecords
            .AsNoTracking()
            .Where(a => a.SectionId == request.SectionId && a.Date == request.Date)
            .ToListAsync(cancellationToken);

        var recordMap = records.ToDictionary(r => r.StudentId);
        var isRecorded = records.Count > 0;

        var studentRows = enrolledStudents
            .OrderBy(s => s.LastName).ThenBy(s => s.FirstName)
            .Select(s =>
            {
                recordMap.TryGetValue(s.Id, out var rec);
                return new StudentAttendanceRow(
                    StudentId: s.Id,
                    FullName: string.IsNullOrEmpty(s.SecondLastName) ? $"{s.FirstName} {s.LastName}" : $"{s.FirstName} {s.LastName} {s.SecondLastName}",
                    StudentCode: s.StudentCode,
                    RecordId: rec?.Id,
                    Status: rec?.Status.ToString() ?? "NotRecorded",
                    Notes: rec?.Notes,
                    ArrivalTime: rec?.ArrivalTime);
            })
            .ToList();

        var present = records.Count(r => r.Status is AttendanceStatus.Present or AttendanceStatus.Remote or AttendanceStatus.HalfDay);
        var total = enrolledStudents.Count;
        var rate = total > 0 ? Math.Round((decimal)present / total * 100, 1) : 0;

        var stats = new AttendanceStats(
            Total: total,
            Present: records.Count(r => r.Status == AttendanceStatus.Present),
            Absent: records.Count(r => r.Status == AttendanceStatus.Absent),
            Late: records.Count(r => r.Status == AttendanceStatus.Late),
            Excused: records.Count(r => r.Status == AttendanceStatus.Excused),
            HalfDay: records.Count(r => r.Status == AttendanceStatus.HalfDay),
            Remote: records.Count(r => r.Status == AttendanceStatus.Remote),
            AttendanceRate: rate);

        return new SectionAttendanceResult(
            SectionId: section.Id,
            SectionName: section.Name,
            GradeLevel: section.GradeLevel.Name,
            Date: request.Date,
            IsRecorded: isRecorded,
            Stats: stats,
            Students: studentRows);
    }
}
