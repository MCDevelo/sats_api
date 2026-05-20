using ErrorOr;
using MediatR;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Attendance.Commands.RecordAttendance;

public record RecordAttendanceCommand(
    Guid SectionId,
    Guid AcademicPeriodId,
    DateOnly Date,
    IReadOnlyList<AttendanceEntry> Entries) : IRequest<ErrorOr<RecordAttendanceResult>>;

public record AttendanceEntry(
    Guid StudentId,
    AttendanceStatus Status,
    string? Notes = null,
    TimeOnly? ArrivalTime = null);

public record RecordAttendanceResult(
    Guid SectionId,
    DateOnly Date,
    int TotalStudents,
    int Present,
    int Absent,
    int Late,
    int Excused,
    IReadOnlyList<AttendanceRecordSummary> Records);

public record AttendanceRecordSummary(
    Guid RecordId,
    Guid StudentId,
    string StudentFullName,
    string Status,
    string? Notes,
    TimeOnly? ArrivalTime);
