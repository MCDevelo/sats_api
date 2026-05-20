using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Guardians.Commands.RemoveStudentGuardian;

public class RemoveStudentGuardianCommandHandler
    : IRequestHandler<RemoveStudentGuardianCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public RemoveStudentGuardianCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(
        RemoveStudentGuardianCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var link = await _db.StudentGuardians
            .FirstOrDefaultAsync(sg =>
                sg.Id == request.StudentGuardianId &&
                sg.TenantId == tenantId, cancellationToken);

        if (link is null)
            return Error.NotFound("StudentGuardian.NotFound", "Vínculo encargado-estudiante no encontrado.");

        // Cannot remove the only primary guardian
        if (link.IsPrimary)
        {
            var otherGuardians = await _db.StudentGuardians
                .CountAsync(sg => sg.StudentId == link.StudentId && sg.Id != link.Id, cancellationToken);

            if (otherGuardians == 0)
                return Error.Conflict("StudentGuardian.OnlyGuardian",
                    "No se puede eliminar al único encargado del estudiante.");
        }

        _db.StudentGuardians.Remove(link);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
