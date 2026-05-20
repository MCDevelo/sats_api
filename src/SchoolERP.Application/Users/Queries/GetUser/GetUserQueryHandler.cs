using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Users.Queries.GetUsers;

namespace SchoolERP.Application.Users.Queries.GetUser;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, ErrorOr<UserResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetUserQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<UserResult>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;
        var now = DateTime.UtcNow;

        var user = await _db.Users
            .Where(u => u.Id == request.UserId && u.TenantId == tenantId)
            .Select(u => new UserResult(
                u.Id,
                u.Email,
                u.Phone,
                u.Role,
                u.IsActive,
                u.EmailVerified,
                u.PhoneVerified,
                u.LastLogin,
                u.LockedUntil != null && u.LockedUntil > now,
                u.LockedUntil,
                u.AvatarUrl,
                u.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            return Error.NotFound("User.NotFound", "Usuario no encontrado.");

        return user;
    }
}
