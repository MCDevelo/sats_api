using ErrorOr;
using MediatR;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Notifications.Commands.SendNotification;

public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, ErrorOr<SendNotificationResult>>
{
    private readonly INotificationDispatcher _dispatcher;
    private readonly ICurrentUserService _currentUser;

    public SendNotificationCommandHandler(INotificationDispatcher dispatcher, ICurrentUserService currentUser)
    {
        _dispatcher = dispatcher;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<SendNotificationResult>> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var notifRequest = new NotificationRequest(
            TenantId: tenantId,
            RecipientUserId: request.RecipientUserId,
            EventType: request.EventType,
            Title: request.Title,
            Body: request.Body,
            Channels: request.Channels,
            Data: request.Data,
            RecipientEmail: request.RecipientEmail);

        await _dispatcher.DispatchAsync(notifRequest, cancellationToken);

        return new SendNotificationResult(
            Dispatched: 1,
            ChannelsSent: request.Channels.Select(c => c.ToString()).ToList());
    }
}
