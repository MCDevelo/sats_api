using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Scheduling.Commands.UpdateScheduleSlot;

public record UpdateScheduleSlotCommand(
    Guid SlotId,
    DayOfWeek Day,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string? Room = null) : IRequest<ErrorOr<Success>>;
