using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = default!;
    public string? DeviceInfo { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public User User { get; private set; } = default!;

    private RefreshToken() { }

    public static RefreshToken Create(
        Guid userId,
        string tokenHash,
        int expiryDays = 30,
        string? deviceInfo = null,
        string? ipAddress = null)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            DeviceInfo = deviceInfo,
            IpAddress = ipAddress,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public bool IsExpired() => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked() => RevokedAt.HasValue;
    public bool IsActive() => !IsExpired() && !IsRevoked();

    public void Revoke()
    {
        RevokedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
