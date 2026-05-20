using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Scheduling.Commands.UpdateScheduleSlot;

public class UpdateScheduleSlotCommandHandler : IRequestHandler<UpdateScheduleSlotCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateScheduleSlotCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateScheduleSlotCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var slot = await _db.ScheduleSlots
            .Include(s => s.TeacherAssignment)
            .FirstOrDefaultAsync(s =>
                s.Id == request.SlotId &&
                s.TenantId == tenantId, cancellationToken);

        if (slot is null)
            return Error.NotFound(description: "Franja horaria no encontrada.");

        // Check conflicts, excluding this slot itself
        var existing = await _db.ScheduleSlots
            .Include(s => s.TeacherAssignment)
            .Where(s =>
                s.TenantId == tenantId &&
                s.Day == request.Day &&
                s.Id != request.SlotId &&
                (s.TeacherAssignment.TeacherId == slot.TeacherAssignment.TeacherId ||
                 s.TeacherAssignment.SectionId == slot.TeacherAssignment.SectionId))
            .ToListAsync(cancellationToken);

        foreach (var other in existing)
        {
            if (!other.OverlapsWith(request.Day, request.StartTime, request.EndTime))
                continue;

            if (other.TeacherAssignment.TeacherId == slot.TeacherAssignment.TeacherId)
                return Error.Conflict(description:
                    $"El docente ya tiene una clase de {other.StartTime:HH:mm} a {other.EndTime:HH:mm} ese día.");

            if (other.TeacherAssignment.SectionId == slot.TeacherAssignment.SectionId)
                return Error.Conflict(description:
                    $"La sección ya tiene clase de {other.StartTime:HH:mm} a {other.EndTime:HH:mm} ese día.");
        }

        slot.Update(request.Day, request.StartTime, request.EndTime, request.Room);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
