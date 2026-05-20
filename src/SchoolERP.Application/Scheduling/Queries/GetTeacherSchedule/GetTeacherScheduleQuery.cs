using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Scheduling.Queries.GetTeacherSchedule;

public record GetTeacherScheduleQuery(
    Guid TeacherId,
    Guid AcademicYearId) : IRequest<ErrorOr<TeacherScheduleResult>>;

public record TeacherScheduleResult(
    Guid TeacherId,
    string TeacherFullName,
    IReadOnlyList<TeacherSlotResult> Slots);

public record TeacherSlotResult(
    Guid SlotId,
    DayOfWeek Day,
    string DayName,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string SubjectName,
    string SectionName,
    string GradeLevel,
    string? Room);
