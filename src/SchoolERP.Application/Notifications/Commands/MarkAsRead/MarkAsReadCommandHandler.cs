using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Notifications.Commands.MarkAsRead;

public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public MarkAsReadCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        var tenantId = _currentUser.TenantId!.Value;

        var notification = await _db.Notifications
            .FirstOrDefaultAsync(n =>
                n.Id == request.NotificationId &&
                n.TenantId == tenantId &&
                n.RecipientUserId == userId, cancellationToken);

        if (notification is null)
            return Error.NotFound(description: "Notificación no encontrada.");

        if (!notification.IsRead)
        {
            notification.MarkRead();
            await _db.SaveChangesAsync(cancellationToken);
        }

        return Result.Success;
    }
}
