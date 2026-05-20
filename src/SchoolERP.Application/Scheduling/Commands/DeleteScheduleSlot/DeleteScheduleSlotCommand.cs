using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Scheduling.Commands.DeleteScheduleSlot;

public record DeleteScheduleSlotCommand(Guid SlotId) : IRequest<ErrorOr<Success>>;
