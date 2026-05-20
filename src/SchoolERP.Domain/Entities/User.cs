using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

public class User : BaseEntity
{
    public Guid TenantId { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool EmailVerified { get; private set; }
    public bool PhoneVerified { get; private set; }
    public DateTime? LastLogin { get; private set; }
    public int FailedAttempts { get; private set; }
    public DateTime? LockedUntil { get; private set; }
    public string PreferredLanguage { get; private set; } = "es";
    public string? AvatarUrl { get; private set; }
    public bool TwoFactorEnabled { get; private set; }
    public string? TwoFactorSecret { get; private set; }

    // Navigation
    public Tenant Tenant { get; private set; } = default!;
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = [];

    private User() { }

    public static User Create(
        Guid tenantId,
        UserRole role,
        string? email = null,
        string? phone = null,
        string? passwordHash = null)
    {
        if (email is null && phone is null)
            throw new ArgumentException("Se requiere email o teléfono.");

        return new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Role = role,
            Email = email?.ToLowerInvariant(),
            Phone = phone,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void RecordLogin()
    {
        LastLogin = DateTime.UtcNow;
        FailedAttempts = 0;
        LockedUntil = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordFailedLogin()
    {
        FailedAttempts++;
        if (FailedAttempts >= 10)
            LockedUntil = DateTime.UtcNow.AddMinutes(15);
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsLockedOut() =>
        LockedUntil.HasValue && LockedUntil > DateTime.UtcNow;

    public void UpdatePasswordHash(string hash)
    {
        PasswordHash = hash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRole(UserRole role)
    {
        Role = role;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateContact(string? email, string? phone)
    {
        if (email is not null) Email = email.ToLowerInvariant();
        if (phone is not null) Phone = phone;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unlock()
    {
        FailedAttempts = 0;
        LockedUntil = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAvatarUrl(string url)
    {
        AvatarUrl = url;
        UpdatedAt = DateTime.UtcNow;
    }
}
