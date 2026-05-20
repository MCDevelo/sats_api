using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Users.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, ErrorOr<CurrentUserResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetCurrentUserQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<CurrentUserResult>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;

        var user = await _db.Users
            .Where(u => u.Id == userId)
            .Select(u => new CurrentUserResult(
                u.Id,
                u.TenantId,
                u.Email,
                u.Phone,
                u.Role,
                u.IsActive,
                u.EmailVerified,
                u.PhoneVerified,
                u.LastLogin,
                u.AvatarUrl,
                u.PreferredLanguage,
                u.TwoFactorEnabled,
                u.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            return Error.NotFound("User.NotFound", "Usuario no encontrado.");

        return user;
    }
}
