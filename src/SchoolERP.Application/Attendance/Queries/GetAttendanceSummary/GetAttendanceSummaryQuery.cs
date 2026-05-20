using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Attendance.Queries.GetAttendanceSummary;

public record GetAttendanceSummaryQuery(
    Guid SectionId,
    Guid AcademicPeriodId) : IRequest<ErrorOr<AttendanceSummaryResult>>;

public record AttendanceSummaryResult(
    Guid SectionId,
    string SectionName,
    string GradeLevel,
    string PeriodName,
    int TotalStudents,
    int TotalSchoolDays,
    decimal AverageAttendanceRate,
    IReadOnlyList<StudentRiskSummary> AtRiskStudents,
    IReadOnlyList<DailyStat> DailyStats);

public record StudentRiskSummary(
    Guid StudentId,
    string FullName,
    string? StudentCode,
    int AbsentDays,
    decimal AbsenceRate,
    string RiskLevel);

public record DailyStat(
    DateOnly Date,
    int Present,
    int Absent,
    int Late,
    int Excused,
    decimal AttendanceRate);
