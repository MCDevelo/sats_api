namespace SchoolERP.Application.Common.Interfaces;

public interface INotificationSender
{
    Task<bool> SendPushAsync(PushMessage message, CancellationToken ct = default);
    Task<bool> SendEmailAsync(EmailMessage message, CancellationToken ct = default);
}

public record PushMessage(
    string DeviceToken,
    string Title,
    string Body,
    Dictionary<string, string>? Data = null);

public record EmailMessage(
    string To,
    string Subject,
    string HtmlBody,
    string? PlainText = null);
