using ErrorOr;
using MediatR;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Notifications.Commands.SendNotification;

public record SendNotificationCommand(
    Guid? RecipientUserId,
    string EventType,
    string Title,
    string Body,
    NotificationChannel[] Channels,
    string? Data = null,
    string? RecipientEmail = null) : IRequest<ErrorOr<SendNotificationResult>>;

public record SendNotificationResult(
    int Dispatched,
    IReadOnlyList<string> ChannelsSent);
