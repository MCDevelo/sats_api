using Microsoft.Extensions.Logging;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Stub implementation. Replace with FirebaseNotificationSender / SesEmailSender
/// once SDK packages are added.
/// </summary>
public class CompositeNotificationSender : INotificationSender
{
    private readonly ILogger<CompositeNotificationSender> _logger;

    public CompositeNotificationSender(ILogger<CompositeNotificationSender> logger)
    {
        _logger = logger;
    }

    public Task<bool> SendPushAsync(PushMessage message, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Push stub → DeviceToken={Token} Title={Title}",
            message.DeviceToken[..Math.Min(8, message.DeviceToken.Length)], message.Title);
        return Task.FromResult(true);
    }

    public Task<bool> SendEmailAsync(EmailMessage message, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Email stub → To={To} Subject={Subject}",
            message.To, message.Subject);
        return Task.FromResult(true);
    }
}
