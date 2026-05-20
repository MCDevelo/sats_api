using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Users.Commands.ResetUserPassword;

public class ResetUserPasswordCommandHandler : IRequestHandler<ResetUserPasswordCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IPasswordService _passwordService;

    public ResetUserPasswordCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        IPasswordService passwordService)
    {
        _db = db;
        _currentUser = currentUser;
        _passwordService = passwordService;
    }

    public async Task<ErrorOr<Success>> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && u.TenantId == tenantId, cancellationToken);

        if (user is null)
            return Error.NotFound("User.NotFound", "Usuario no encontrado.");

        user.UpdatePasswordHash(_passwordService.Hash(request.NewPassword));
        user.Unlock(); // clear any lockout when admin resets password

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}
