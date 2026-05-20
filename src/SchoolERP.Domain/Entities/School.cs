using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

public class School : BaseEntity
{
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = default!;
    public string? LegalName { get; private set; }
    public string? CodeMinerd { get; private set; }
    public string? Rnc { get; private set; }
    public string? Address { get; private set; }
    public string? Sector { get; private set; }
    public string? Municipality { get; private set; }
    public string? Province { get; private set; }
    public string? PhonePrimary { get; private set; }
    public string? PhoneSecondary { get; private set; }
    public string? Email { get; private set; }
    public string? Website { get; private set; }
    public string? LogoUrl { get; private set; }
    public string? SealUrl { get; private set; }
    public EducationLevel LevelType { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation
    public Tenant Tenant { get; private set; } = default!;
    public ICollection<AcademicYear> AcademicYears { get; private set; } = [];
    public ICollection<Student> Students { get; private set; } = [];
    public ICollection<Teacher> Teachers { get; private set; } = [];
    public ICollection<Section> Sections { get; private set; } = [];

    private School() { }

    public static School Create(
        Guid tenantId,
        string name,
        EducationLevel levelType,
        string? codeMinerd = null,
        string? email = null,
        string? phonePrimary = null,
        string? province = null,
        string? municipality = null)
    {
        return new School
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = name,
            LevelType = levelType,
            CodeMinerd = codeMinerd,
            Email = email,
            PhonePrimary = phonePrimary,
            Province = province,
            Municipality = municipality,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string? email, string? phone, string? address)
    {
        Name = name;
        Email = email;
        PhonePrimary = phone;
        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate() => IsActive = false;
}
