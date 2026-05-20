using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Teachers.Queries.GetTeacherWorkload;

public record GetTeacherWorkloadQuery(
    Guid TeacherId,
    Guid? AcademicYearId = null) : IRequest<ErrorOr<TeacherWorkloadResult>>;

public record TeacherWorkloadResult(
    Guid TeacherId,
    string TeacherName,
    string? NationalId,
    string? AcademicYear,
    double TotalWeeklyHours,
    int MaxWeeklyHours,
    bool IsOverloaded,
    IReadOnlyList<WorkloadSectionItem> Sections);

public record WorkloadSectionItem(
    Guid AssignmentId,
    string GradeLevel,
    string Section,
    string Subject,
    string Shift,
    double WeeklyHours,
    int Students);
