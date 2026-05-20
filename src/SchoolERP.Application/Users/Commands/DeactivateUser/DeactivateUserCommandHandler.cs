using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Users.Commands.DeactivateUser;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeactivateUserCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;
        var requesterId = _currentUser.UserId!.Value;

        if (request.UserId == requesterId)
            return Error.Conflict("User.CannotDeactivateSelf", "No puedes desactivar tu propia cuenta.");

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && u.TenantId == tenantId, cancellationToken);

        if (user is null)
            return Error.NotFound("User.NotFound", "Usuario no encontrado.");

        if (!user.IsActive)
            return Error.Conflict("User.AlreadyInactive", "El usuario ya está desactivado.");

        user.Deactivate();
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}
