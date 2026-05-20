using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Notifications.Queries.GetUnreadCount;

public class GetUnreadCountQueryHandler : IRequestHandler<GetUnreadCountQuery, ErrorOr<int>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetUnreadCountQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<int>> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var tenantId = _currentUser.TenantId!.Value;

        var count = await _db.Notifications
            .CountAsync(n =>
                n.TenantId == tenantId &&
                n.RecipientUserId == userId &&
                n.Channel == "InApp" &&
                !n.IsRead,
                cancellationToken);

        return count;
    }
}
