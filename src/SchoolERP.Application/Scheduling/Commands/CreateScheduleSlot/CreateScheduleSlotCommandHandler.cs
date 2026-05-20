using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Scheduling.Commands.CreateScheduleSlot;

public class CreateScheduleSlotCommandHandler : IRequestHandler<CreateScheduleSlotCommand, ErrorOr<Guid>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateScheduleSlotCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateScheduleSlotCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var assignment = await _db.TeacherAssignments
            .Include(a => a.Section)
            .FirstOrDefaultAsync(a =>
                a.Id == request.TeacherAssignmentId &&
                a.Section.TenantId == tenantId &&
                a.IsActive, cancellationToken);

        if (assignment is null)
            return Error.NotFound(description: "Asignación docente no encontrada.");

        var conflict = await DetectConflict(
            tenantId,
            assignment.TeacherId,
            assignment.SectionId,
            request.Day,
            request.StartTime,
            request.EndTime,
            excludeSlotId: null,
            cancellationToken);

        if (conflict is not null)
            return conflict.Value; // Error? → unwrap the Error struct

        var slot = ScheduleSlot.Create(
            tenantId,
            request.TeacherAssignmentId,
            request.Day,
            request.StartTime,
            request.EndTime,
            request.Room);

        _db.ScheduleSlots.Add(slot);
        await _db.SaveChangesAsync(cancellationToken);

        return slot.Id;
    }

    private async Task<Error?> DetectConflict(
        Guid tenantId,
        Guid teacherId,
        Guid sectionId,
        DayOfWeek day,
        TimeOnly start,
        TimeOnly end,
        Guid? excludeSlotId,
        CancellationToken cancellationToken)
    {
        var existing = await _db.ScheduleSlots
            .Include(s => s.TeacherAssignment)
            .Where(s =>
                s.TenantId == tenantId &&
                s.Day == day &&
                (excludeSlotId == null || s.Id != excludeSlotId) &&
                (s.TeacherAssignment.TeacherId == teacherId ||
                 s.TeacherAssignment.SectionId == sectionId))
            .ToListAsync(cancellationToken);

        foreach (var slot in existing)
        {
            if (!slot.OverlapsWith(day, start, end))
                continue;

            if (slot.TeacherAssignment.TeacherId == teacherId)
                return Error.Conflict(description:
                    $"El docente ya tiene una clase de {slot.StartTime:HH:mm} a {slot.EndTime:HH:mm} ese día.");

            if (slot.TeacherAssignment.SectionId == sectionId)
                return Error.Conflict(description:
                    $"La sección ya tiene clase de {slot.StartTime:HH:mm} a {slot.EndTime:HH:mm} ese día.");
        }

        return null;
    }
}
