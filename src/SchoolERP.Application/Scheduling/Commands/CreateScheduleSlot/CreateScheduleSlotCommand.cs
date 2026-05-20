using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Scheduling.Commands.CreateScheduleSlot;

public record CreateScheduleSlotCommand(
    Guid TeacherAssignmentId,
    DayOfWeek Day,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string? Room = null) : IRequest<ErrorOr<Guid>>;
