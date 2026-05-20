using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Attendance.Commands.RecordAttendance;

public class RecordAttendanceCommandHandler : IRequestHandler<RecordAttendanceCommand, ErrorOr<RecordAttendanceResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public RecordAttendanceCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<RecordAttendanceResult>> Handle(RecordAttendanceCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;
        var teacherId = _currentUser.UserId;

        var section = await _db.Sections
            .FirstOrDefaultAsync(s => s.Id == request.SectionId && s.TenantId == tenantId && s.IsActive, cancellationToken);

        if (section is null)
            return Error.NotFound(description: "Sección no encontrada.");

        var period = await _db.AcademicPeriods
            .FirstOrDefaultAsync(p => p.Id == request.AcademicPeriodId && p.IsActive, cancellationToken);

        if (period is null)
            return Error.NotFound(description: "Período académico no encontrado.");

        if (request.Date < DateOnly.FromDateTime(period.StartDate) || request.Date > DateOnly.FromDateTime(period.EndDate))
            return Error.Validation(description: "La fecha no corresponde al período académico indicado.");

        // Validate all students are actively enrolled in this section
        var studentIds = request.Entries.Select(e => e.StudentId).ToList();

        var enrolledStudentIds = await _db.Enrollments
            .Where(e => e.SectionId == request.SectionId && e.Status == EnrollmentStatus.Active && studentIds.Contains(e.StudentId))
            .Select(e => e.StudentId)
            .ToListAsync(cancellationToken);

        var notEnrolled = studentIds.Except(enrolledStudentIds).ToList();
        if (notEnrolled.Count > 0)
            return Error.Validation(description: $"Estudiante(s) no matriculado(s) en esta sección: {string.Join(", ", notEnrolled)}.");

        // Load existing records for this section/date to upsert
        var existingRecords = await _db.AttendanceRecords
            .Where(a => a.SectionId == request.SectionId && a.Date == request.Date)
            .ToListAsync(cancellationToken);

        // Load student names for the result
        var students = await _db.Students
            .AsNoTracking()
            .Where(s => studentIds.Contains(s.Id))
            .Select(s => new { s.Id, s.FirstName, s.LastName, s.SecondLastName })
            .ToDictionaryAsync(s => s.Id, cancellationToken);

        var savedRecords = new List<AttendanceRecord>();

        foreach (var entry in request.Entries)
        {
            var existing = existingRecords.FirstOrDefault(r => r.StudentId == entry.StudentId);

            if (existing is not null)
            {
                existing.Update(entry.Status, entry.Notes);
                savedRecords.Add(existing);
            }
            else
            {
                var record = AttendanceRecord.Create(
                    tenantId: tenantId,
                    studentId: entry.StudentId,
                    sectionId: request.SectionId,
                    academicPeriodId: request.AcademicPeriodId,
                    date: request.Date,
                    status: entry.Status,
                    teacherId: teacherId,
                    notes: entry.Notes,
                    arrivalTime: entry.ArrivalTime);

                _db.AttendanceRecords.Add(record);
                savedRecords.Add(record);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        var summaries = savedRecords.Select(r => new AttendanceRecordSummary(
            RecordId: r.Id,
            StudentId: r.StudentId,
            StudentFullName: students.TryGetValue(r.StudentId, out var s)
                ? string.IsNullOrEmpty(s.SecondLastName) ? $"{s.FirstName} {s.LastName}" : $"{s.FirstName} {s.LastName} {s.SecondLastName}"
                : string.Empty,
            Status: r.Status.ToString(),
            Notes: r.Notes,
            ArrivalTime: r.ArrivalTime)).ToList();

        return new RecordAttendanceResult(
            SectionId: request.SectionId,
            Date: request.Date,
            TotalStudents: savedRecords.Count,
            Present: savedRecords.Count(r => r.Status == AttendanceStatus.Present || r.Status == AttendanceStatus.HalfDay || r.Status == AttendanceStatus.Remote),
            Absent: savedRecords.Count(r => r.Status == AttendanceStatus.Absent),
            Late: savedRecords.Count(r => r.Status == AttendanceStatus.Late),
            Excused: savedRecords.Count(r => r.Status == AttendanceStatus.Excused),
            Records: summaries);
    }
}
