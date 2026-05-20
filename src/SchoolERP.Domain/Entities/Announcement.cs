using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

public class Announcement : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid SchoolId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string Title { get; private set; } = default!;
    public string Body { get; private set; } = default!;
    public AnnouncementAudience Audience { get; private set; }
    public Guid? AudienceId { get; private set; } // SectionId or GradeLevelId when Audience ≠ All/Staff/Parents
    public AnnouncementPriority Priority { get; private set; }
    public bool IsPublished { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    // Navigation
    public Tenant Tenant { get; private set; } = default!;
    public School School { get; private set; } = default!;
    public User Author { get; private set; } = default!;

    private Announcement() { }

    public static Announcement Create(
        Guid tenantId,
        Guid schoolId,
        Guid authorId,
        string title,
        string body,
        AnnouncementAudience audience,
        AnnouncementPriority priority,
        Guid? audienceId = null,
        DateTime? expiresAt = null)
    {
        return new Announcement
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            SchoolId = schoolId,
            AuthorId = authorId,
            Title = title,
            Body = body,
            Audience = audience,
            AudienceId = audienceId,
            Priority = priority,
            IsPublished = false,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Publish()
    {
        IsPublished = true;
        PublishedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string title, string body, AnnouncementPriority priority, DateTime? expiresAt)
    {
        Title = title;
        Body = body;
        Priority = priority;
        ExpiresAt = expiresAt;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsActive => IsPublished && (ExpiresAt == null || ExpiresAt > DateTime.UtcNow);
}
