using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string LegalName { get; private set; } = default!;
    public string? Rnc { get; private set; }
    public string Country { get; private set; } = "DO";
    public string ContactEmail { get; private set; } = default!;
    public string? ContactPhone { get; private set; }
    public string Plan { get; private set; } = "trial";
    public bool IsActive { get; private set; } = true;
    public DateTime? TrialEndsAt { get; private set; }
    public DateTime ContractStart { get; private set; } = DateTime.UtcNow;
    public DateTime? ContractEnd { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? PrimaryColor { get; private set; }
    public bool OnboardingCompleted { get; private set; }
    public int OnboardingStep { get; private set; } = 1;

    // Navigation
    public ICollection<School> Schools { get; private set; } = [];

    private Tenant() { }

    public static Tenant Create(
        string name,
        string legalName,
        string contactEmail,
        string? rnc = null,
        string? contactPhone = null)
    {
        return new Tenant
        {
            Id = Guid.NewGuid(),
            Name = name,
            LegalName = legalName,
            ContactEmail = contactEmail,
            Rnc = rnc,
            ContactPhone = contactPhone,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string name,
        string legalName,
        string contactEmail,
        string? rnc,
        string? contactPhone,
        string? logoUrl,
        string? primaryColor)
    {
        Name = name;
        LegalName = legalName;
        ContactEmail = contactEmail;
        Rnc = rnc;
        ContactPhone = contactPhone;
        LogoUrl = logoUrl;
        PrimaryColor = primaryColor;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePlan(string plan)
    {
        Plan = plan;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CompleteOnboarding()
    {
        OnboardingCompleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate() => IsActive = false;
}
