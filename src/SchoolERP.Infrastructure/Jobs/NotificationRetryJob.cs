using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Infrastructure.Jobs;

/// <summary>
/// Retries unsent external notifications (Email / Push) that have failed fewer than 3 times.
/// Scheduled: every 30 minutes.
/// </summary>
public class NotificationRetryJob
{
    private const int MaxRetries = 3;

    private readonly IApplicationDbContext _db;
    private readonly INotificationSender _sender;
    private readonly ILogger<NotificationRetryJob> _logger;

    public NotificationRetryJob(
        IApplicationDbContext db,
        INotificationSender sender,
        ILogger<NotificationRetryJob> logger)
    {
        _db = db;
        _sender = sender;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var pending = await _db.Notifications
            .Include(n => n.RecipientUser)
            .Where(n =>
                !n.IsSent &&
                n.RetryCount < MaxRetries &&
                (n.Channel == "Email" || n.Channel == "Push"))
            .ToListAsync();

        if (pending.Count == 0)
            return;

        _logger.LogInformation(
            "NotificationRetryJob: retrying {Count} notification(s).", pending.Count);

        var succeeded = 0;
        var failed    = 0;

        foreach (var notification in pending)
        {
            try
            {
                var sent = notification.Channel switch
                {
                    "Email" when notification.RecipientUser?.Email is not null =>
                        await _sender.SendEmailAsync(new EmailMessage(
                            To: notification.RecipientUser.Email,
                            Subject: notification.Title,
                            HtmlBody: notification.Body)),

                    "Push" => false, // Push requires device token not stored on notification; skip

                    _ => false
                };

                if (sent)
                {
                    notification.MarkSent();
                    succeeded++;
                }
                else
                {
                    notification.RecordFailure("Retry skipped: missing contact info or unsupported channel.");
                    failed++;
                }
            }
            catch (Exception ex)
            {
                notification.RecordFailure(ex.Message[..Math.Min(500, ex.Message.Length)]);
                failed++;
                _logger.LogWarning(ex,
                    "NotificationRetryJob: failed to send notification {Id}.", notification.Id);
            }
        }

        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "NotificationRetryJob: completed — succeeded={Succeeded}, failed={Failed}.",
            succeeded, failed);
    }
}
