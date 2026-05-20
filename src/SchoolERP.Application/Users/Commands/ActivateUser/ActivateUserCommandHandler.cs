using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Users.Commands.ActivateUser;

public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public ActivateUserCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && u.TenantId == tenantId, cancellationToken);

        if (user is null)
            return Error.NotFound("User.NotFound", "Usuario no encontrado.");

        if (user.IsActive)
            return Error.Conflict("User.AlreadyActive", "El usuario ya está activo.");

        user.Activate();
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}
