using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Scheduling.Queries.GetSectionSchedule;

public record GetSectionScheduleQuery(
    Guid SectionId,
    Guid AcademicYearId) : IRequest<ErrorOr<SectionScheduleResult>>;

public record SectionScheduleResult(
    Guid SectionId,
    string SectionName,
    string GradeLevel,
    IReadOnlyList<ScheduleSlotResult> Slots);

public record ScheduleSlotResult(
    Guid SlotId,
    DayOfWeek Day,
    string DayName,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string SubjectName,
    string TeacherFullName,
    string? Room);
