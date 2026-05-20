using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Scheduling.Commands.DeleteScheduleSlot;

public class DeleteScheduleSlotCommandHandler : IRequestHandler<DeleteScheduleSlotCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteScheduleSlotCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteScheduleSlotCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var slot = await _db.ScheduleSlots
            .FirstOrDefaultAsync(s =>
                s.Id == request.SlotId &&
                s.TenantId == tenantId, cancellationToken);

        if (slot is null)
            return Error.NotFound(description: "Franja horaria no encontrada.");

        _db.ScheduleSlots.Remove(slot);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
