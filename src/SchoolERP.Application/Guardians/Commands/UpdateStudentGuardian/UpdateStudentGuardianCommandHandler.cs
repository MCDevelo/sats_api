using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Guardians.Commands.LinkGuardianToStudent;

namespace SchoolERP.Application.Guardians.Commands.UpdateStudentGuardian;

public class UpdateStudentGuardianCommandHandler
    : IRequestHandler<UpdateStudentGuardianCommand, ErrorOr<StudentGuardianResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateStudentGuardianCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<StudentGuardianResult>> Handle(
        UpdateStudentGuardianCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var link = await _db.StudentGuardians
            .Include(sg => sg.Student)
            .Include(sg => sg.Guardian)
            .FirstOrDefaultAsync(sg =>
                sg.Id == request.StudentGuardianId &&
                sg.TenantId == tenantId, cancellationToken);

        if (link is null)
            return Error.NotFound("StudentGuardian.NotFound", "Vínculo encargado-estudiante no encontrado.");

        // If promoting to primary, demote any existing primary
        if (request.IsPrimary && !link.IsPrimary)
        {
            var existingPrimary = await _db.StudentGuardians
                .Where(sg =>
                    sg.StudentId == link.StudentId &&
                    sg.IsPrimary &&
                    sg.Id != link.Id)
                .ToListAsync(cancellationToken);

            foreach (var ep in existingPrimary)
                ep.Update(ep.Relationship, isPrimary: false, ep.CanPickup,
                    ep.IsEmergencyContact, ep.ReceivesNotifications, ep.HasCustodyOrder, ep.CustodyNotes);
        }

        link.Update(
            relationship: request.Relationship.ToLowerInvariant(),
            isPrimary: request.IsPrimary,
            canPickup: request.CanPickup,
            isEmergencyContact: request.IsEmergencyContact,
            receivesNotifications: request.ReceivesNotifications,
            hasCustodyOrder: request.HasCustodyOrder,
            custodyNotes: request.CustodyNotes);

        await _db.SaveChangesAsync(cancellationToken);

        return LinkGuardianToStudentCommandHandler.ToResult(link, link.Student, link.Guardian);
    }
}
