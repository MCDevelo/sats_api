using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Guardians.Commands.LinkUserToGuardian;

public class LinkUserToGuardianCommandHandler
    : IRequestHandler<LinkUserToGuardianCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public LinkUserToGuardianCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(
        LinkUserToGuardianCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var guardian = await _db.Guardians
            .FirstOrDefaultAsync(g => g.Id == request.GuardianId && g.TenantId == tenantId, cancellationToken);

        if (guardian is null)
            return Error.NotFound("Guardian.NotFound", "Encargado no encontrado.");

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && u.TenantId == tenantId && u.IsActive, cancellationToken);

        if (user is null)
            return Error.NotFound("User.NotFound", "Usuario no encontrado.");

        // Ensure no other guardian in this tenant is already linked to this user
        var userAlreadyLinked = await _db.Guardians
            .AnyAsync(g => g.UserId == request.UserId && g.Id != request.GuardianId, cancellationToken);

        if (userAlreadyLinked)
            return Error.Conflict("Guardian.UserAlreadyLinked",
                "Este usuario ya está vinculado a otro encargado.");

        guardian.LinkUser(request.UserId);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
