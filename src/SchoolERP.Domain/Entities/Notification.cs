using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid? RecipientUserId { get; private set; }
    public string Channel { get; private set; } = default!; // Push, Email, SMS, WhatsApp, InApp
    public string EventType { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public string Body { get; private set; } = default!;
    public string? Data { get; private set; } // JSON payload
    public bool IsSent { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? SentAt { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int RetryCount { get; private set; }

    // Navigation
    public Tenant Tenant { get; private set; } = default!;
    public User? RecipientUser { get; private set; }

    private Notification() { }

    public static Notification Create(
        Guid tenantId,
        string channel,
        string eventType,
        string title,
        string body,
        Guid? recipientUserId = null,
        string? data = null)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Channel = channel,
            EventType = eventType,
            Title = title,
            Body = body,
            RecipientUserId = recipientUserId,
            Data = data,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void MarkSent()
    {
        IsSent = true;
        SentAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkRead()
    {
        IsRead = true;
        ReadAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordFailure(string errorMessage)
    {
        ErrorMessage = errorMessage;
        RetryCount++;
        UpdatedAt = DateTime.UtcNow;
    }
}
