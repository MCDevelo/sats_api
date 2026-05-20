using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Models;

namespace SchoolERP.Application.Notifications.Queries.GetUserNotifications;

public class GetUserNotificationsQueryHandler
    : IRequestHandler<GetUserNotificationsQuery, ErrorOr<PagedResult<NotificationResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetUserNotificationsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<PagedResult<NotificationResult>>> Handle(
        GetUserNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var tenantId = _currentUser.TenantId!.Value;

        var query = _db.Notifications
            .Where(n =>
                n.TenantId == tenantId &&
                n.RecipientUserId == userId &&
                n.Channel == "InApp");

        if (request.OnlyUnread == true)
            query = query.Where(n => !n.IsRead);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(n => new NotificationResult(
                n.Id,
                n.EventType,
                n.Title,
                n.Body,
                n.Data,
                n.IsRead,
                n.ReadAt,
                n.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<NotificationResult>(items, total, request.Page, request.PageSize);
    }
}
