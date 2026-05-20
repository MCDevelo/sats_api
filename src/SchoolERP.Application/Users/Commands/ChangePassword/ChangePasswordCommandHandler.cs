using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Users.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IPasswordService _passwordService;

    public ChangePasswordCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        IPasswordService passwordService)
    {
        _db = db;
        _currentUser = currentUser;
        _passwordService = passwordService;
    }

    public async Task<ErrorOr<Success>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
            return Error.NotFound("User.NotFound", "Usuario no encontrado.");

        if (user.PasswordHash is null || !_passwordService.Verify(request.CurrentPassword, user.PasswordHash))
            return Error.Unauthorized("User.InvalidPassword", "Contraseña actual incorrecta.");

        user.UpdatePasswordHash(_passwordService.Hash(request.NewPassword));
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}
