using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Attendance.Queries.GetStudentAttendance;

public record GetStudentAttendanceQuery(
    Guid StudentId,
    Guid AcademicPeriodId) : IRequest<ErrorOr<StudentAttendanceResult>>;

public record StudentAttendanceResult(
    Guid StudentId,
    string StudentFullName,
    string? StudentCode,
    Guid AcademicPeriodId,
    string PeriodName,
    AttendanceSummary Summary,
    IReadOnlyList<DailyAttendanceRecord> Records);

public record AttendanceSummary(
    int TotalSchoolDays,
    int Present,
    int Absent,
    int Late,
    int Excused,
    int HalfDay,
    int Remote,
    decimal AttendanceRate,
    decimal AbsenceRate,
    RiskLevel RiskLevel,
    string RiskDescription);

public record DailyAttendanceRecord(
    Guid RecordId,
    DateOnly Date,
    string Status,
    string? Notes,
    TimeOnly? ArrivalTime);

public enum RiskLevel { Low, Medium, High, Critical }
