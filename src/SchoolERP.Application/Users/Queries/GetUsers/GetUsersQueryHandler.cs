using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Models;

namespace SchoolERP.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, ErrorOr<PagedResult<UserResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetUsersQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<PagedResult<UserResult>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;
        var now = DateTime.UtcNow;

        var query = _db.Users.Where(u => u.TenantId == tenantId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(u =>
                (u.Email != null && u.Email.Contains(search)) ||
                (u.Phone != null && u.Phone.Contains(search)));
        }

        if (request.Role.HasValue)
            query = query.Where(u => u.Role == request.Role.Value);

        if (request.IsActive.HasValue)
            query = query.Where(u => u.IsActive == request.IsActive.Value);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(u => u.Email)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
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
            .ToListAsync(cancellationToken);

        return new PagedResult<UserResult>(items, total, request.Page, request.PageSize);
    }
}
