using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;
using RefreshTokenEntity = SchoolERP.Domain.Entities.RefreshToken;

namespace SchoolERP.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ErrorOr<RefreshTokenResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly IJwtService _jwtService;

    public RefreshTokenCommandHandler(IApplicationDbContext db, IJwtService jwtService)
    {
        _db = db;
        _jwtService = jwtService;
    }

    public async Task<ErrorOr<RefreshTokenResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = Convert.ToBase64String(
            System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(request.Token)));

        var storedToken = await _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);

        if (storedToken is null || !storedToken.IsActive())
            return Error.Unauthorized(description: "Token de actualización inválido o expirado.");

        if (!storedToken.User.IsActive)
            return Error.Unauthorized(description: "Cuenta inactiva.");

        // Rotate the refresh token
        storedToken.Revoke();

        var rawNewToken = _jwtService.GenerateRefreshToken();
        var newTokenHash = Convert.ToBase64String(
            System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(rawNewToken)));

        var newRefreshToken = RefreshTokenEntity.Create(
            userId: storedToken.UserId,
            tokenHash: newTokenHash,
            deviceInfo: request.DeviceInfo,
            ipAddress: request.IpAddress);

        _db.RefreshTokens.Add(newRefreshToken);
        await _db.SaveChangesAsync(cancellationToken);

        var accessToken = _jwtService.GenerateAccessToken(storedToken.User);

        return new RefreshTokenResult(
            AccessToken: accessToken,
            RefreshToken: rawNewToken,
            ExpiresAt: DateTime.UtcNow.AddMinutes(15));
    }
}
