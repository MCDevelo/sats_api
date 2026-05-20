using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Attendance.Queries.GetSectionAttendance;

public record GetSectionAttendanceQuery(
    Guid SectionId,
    DateOnly Date) : IRequest<ErrorOr<SectionAttendanceResult>>;

public record SectionAttendanceResult(
    Guid SectionId,
    string SectionName,
    string GradeLevel,
    DateOnly Date,
    bool IsRecorded,
    AttendanceStats Stats,
    IReadOnlyList<StudentAttendanceRow> Students);

public record AttendanceStats(
    int Total,
    int Present,
    int Absent,
    int Late,
    int Excused,
    int HalfDay,
    int Remote,
    decimal AttendanceRate);

public record StudentAttendanceRow(
    Guid StudentId,
    string FullName,
    string? StudentCode,
    Guid? RecordId,
    string Status,
    string? Notes,
    TimeOnly? ArrivalTime);
