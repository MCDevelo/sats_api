using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

public class Message : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid SenderId { get; private set; }
    public Guid RecipientId { get; private set; }
    public string Subject { get; private set; } = default!;
    public string Body { get; private set; } = default!;
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public Guid? ParentMessageId { get; private set; } // thread reply

    // Navigation
    public Tenant Tenant { get; private set; } = default!;
    public User Sender { get; private set; } = default!;
    public User Recipient { get; private set; } = default!;
    public Message? ParentMessage { get; private set; }

    private Message() { }

    public static Message Create(
        Guid tenantId,
        Guid senderId,
        Guid recipientId,
        string subject,
        string body,
        Guid? parentMessageId = null)
    {
        return new Message
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            SenderId = senderId,
            RecipientId = recipientId,
            Subject = subject,
            Body = body,
            IsRead = false,
            ParentMessageId = parentMessageId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void MarkRead()
    {
        if (IsRead) return;
        IsRead = true;
        ReadAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
