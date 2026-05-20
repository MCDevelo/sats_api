using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Notifications.Commands.MarkAllAsRead;

public class MarkAllAsReadCommandHandler : IRequestHandler<MarkAllAsReadCommand, ErrorOr<int>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public MarkAllAsReadCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<int>> Handle(MarkAllAsReadCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var tenantId = _currentUser.TenantId!.Value;

        var unread = await _db.Notifications
            .Where(n =>
                n.TenantId == tenantId &&
                n.RecipientUserId == userId &&
                !n.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var notification in unread)
            notification.MarkRead();

        if (unread.Count > 0)
            await _db.SaveChangesAsync(cancellationToken);

        return unread.Count;
    }
}
