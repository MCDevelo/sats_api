using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;

    public LogoutCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ErrorOr<Success>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = Convert.ToBase64String(
            System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(request.RefreshToken)));

        var storedToken = await _db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);

        if (storedToken is not null && storedToken.IsActive())
        {
            storedToken.Revoke();
            await _db.SaveChangesAsync(cancellationToken);
        }

        return Result.Success;
    }
}
