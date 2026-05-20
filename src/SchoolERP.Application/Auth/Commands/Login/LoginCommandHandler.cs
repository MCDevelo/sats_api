using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;
using RefreshTokenEntity = SchoolERP.Domain.Entities.RefreshToken;

namespace SchoolERP.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ErrorOr<LoginResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly IJwtService _jwtService;
    private readonly IPasswordService _passwordService;

    public LoginCommandHandler(
        IApplicationDbContext db,
        IJwtService jwtService,
        IPasswordService passwordService)
    {
        _db = db;
        _jwtService = jwtService;
        _passwordService = passwordService;
    }

    public async Task<ErrorOr<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var emailOrPhone = request.EmailOrPhone.Trim().ToLowerInvariant();

        var user = await _db.Users
            .FirstOrDefaultAsync(u =>
                (u.Email == emailOrPhone || u.Phone == request.EmailOrPhone.Trim()) &&
                u.IsActive,
                cancellationToken);

        if (user is null)
            return Error.Unauthorized(description: "Credenciales inválidas.");

        if (user.IsLockedOut())
            return Error.Unauthorized(description: "Cuenta bloqueada temporalmente. Intente en unos minutos.");

        if (user.PasswordHash is null || !_passwordService.Verify(request.Password, user.PasswordHash))
        {
            user.RecordFailedLogin();
            await _db.SaveChangesAsync(cancellationToken);
            return Error.Unauthorized(description: "Credenciales inválidas.");
        }

        user.RecordLogin();

        var rawRefreshToken = _jwtService.GenerateRefreshToken();
        var tokenHash = Convert.ToBase64String(
            System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(rawRefreshToken)));

        var refreshToken = RefreshTokenEntity.Create(
            userId: user.Id,
            tokenHash: tokenHash,
            deviceInfo: request.DeviceInfo,
            ipAddress: request.IpAddress);

        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync(cancellationToken);

        var accessToken = _jwtService.GenerateAccessToken(user);

        return new LoginResult(
            AccessToken: accessToken,
            RefreshToken: rawRefreshToken,
            ExpiresAt: DateTime.UtcNow.AddMinutes(15),
            User: new UserInfo(
                Id: user.Id,
                TenantId: user.TenantId,
                Email: user.Email,
                Phone: user.Phone,
                Role: user.Role.ToString(),
                PreferredLanguage: user.PreferredLanguage,
                AvatarUrl: user.AvatarUrl));
    }
}
