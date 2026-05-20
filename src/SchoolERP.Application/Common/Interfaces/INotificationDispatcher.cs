namespace SchoolERP.Application.Common.Interfaces;

/// <summary>
/// Dispatches domain events to the notification pipeline.
/// Resolves channels (Push/Email/InApp) based on user preferences and event type.
/// </summary>
public interface INotificationDispatcher
{
    Task DispatchAsync(NotificationRequest request, CancellationToken ct = default);
    Task DispatchBulkAsync(IEnumerable<NotificationRequest> requests, CancellationToken ct = default);
}

public record NotificationRequest(
    Guid TenantId,
    Guid? RecipientUserId,
    string EventType,
    string Title,
    string Body,
    NotificationChannel[] Channels,
    string? Data = null,
    string? RecipientEmail = null,
    string? RecipientDeviceToken = null);

public enum NotificationChannel { InApp, Push, Email, SMS, WhatsApp }
