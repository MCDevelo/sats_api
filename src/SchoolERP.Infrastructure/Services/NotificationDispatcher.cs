using Microsoft.Extensions.Logging;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

public class NotificationDispatcher : INotificationDispatcher
{
    private readonly IApplicationDbContext _db;
    private readonly INotificationSender _sender;
    private readonly ILogger<NotificationDispatcher> _logger;

    public NotificationDispatcher(
        IApplicationDbContext db,
        INotificationSender sender,
        ILogger<NotificationDispatcher> logger)
    {
        _db = db;
        _sender = sender;
        _logger = logger;
    }

    public async Task DispatchAsync(NotificationRequest request, CancellationToken ct = default)
    {
        foreach (var channel in request.Channels)
        {
            var notification = Notification.Create(
                tenantId: request.TenantId,
                channel: channel.ToString(),
                eventType: request.EventType,
                title: request.Title,
                body: request.Body,
                recipientUserId: request.RecipientUserId,
                data: request.Data);

            _db.Notifications.Add(notification);

            var sent = channel switch
            {
                NotificationChannel.Push when request.RecipientDeviceToken is not null =>
                    await SendPushSafe(request, ct),
                NotificationChannel.Email when request.RecipientEmail is not null =>
                    await SendEmailSafe(request, ct),
                NotificationChannel.InApp => true, // persisted above, no external dispatch needed
                _ => false
            };

            if (sent)
                notification.MarkSent();
            else if (channel != NotificationChannel.InApp)
                notification.RecordFailure($"Channel {channel} could not be dispatched.");
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task DispatchBulkAsync(IEnumerable<NotificationRequest> requests, CancellationToken ct = default)
    {
        foreach (var request in requests)
            await DispatchAsync(request, ct);
    }

    private async Task<bool> SendPushSafe(NotificationRequest request, CancellationToken ct)
    {
        try
        {
            return await _sender.SendPushAsync(new PushMessage(
                DeviceToken: request.RecipientDeviceToken!,
                Title: request.Title,
                Body: request.Body), ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Push notification failed for event {EventType}", request.EventType);
            return false;
        }
    }

    private async Task<bool> SendEmailSafe(NotificationRequest request, CancellationToken ct)
    {
        try
        {
            return await _sender.SendEmailAsync(new EmailMessage(
                To: request.RecipientEmail!,
                Subject: request.Title,
                HtmlBody: request.Body), ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Email notification failed for event {EventType}", request.EventType);
            return false;
        }
    }
}
