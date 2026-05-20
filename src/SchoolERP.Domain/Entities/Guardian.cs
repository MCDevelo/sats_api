using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

public class Guardian : BaseEntity
{
    public Guid TenantId { get; private set; }
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string? NationalId { get; private set; }       // Cédula RD (11 dígitos)
    public string? Email { get; private set; }
    public string? Phone { get; private set; }            // Teléfono primario
    public string? PhoneSecondary { get; private set; }   // Teléfono secundario
    public string? WhatsApp { get; private set; }
    public string? Address { get; private set; }
    public string? Occupation { get; private set; }
    public string? Workplace { get; private set; }
    public bool IsFinancialResponsible { get; private set; }
    public Gender? Gender { get; private set; }
    public Guid? UserId { get; private set; }             // Portal de padres

    // Navigation
    public Tenant Tenant { get; private set; } = default!;
    public User? User { get; private set; }
    public ICollection<StudentGuardian> StudentGuardians { get; private set; } = [];

    private Guardian() { }

    public static Guardian Create(
        Guid tenantId,
        string firstName,
        string lastName,
        string? email = null,
        string? phone = null,
        string? nationalId = null,
        string? phoneSecondary = null,
        string? whatsApp = null,
        string? address = null,
        string? occupation = null,
        string? workplace = null,
        bool isFinancialResponsible = false,
        Gender? gender = null)
    {
        return new Guardian
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            FirstName = firstName,
            LastName = lastName,
            Email = email?.ToLowerInvariant(),
            Phone = phone,
            PhoneSecondary = phoneSecondary,
            WhatsApp = whatsApp,
            NationalId = nationalId,
            Address = address,
            Occupation = occupation,
            Workplace = workplace,
            IsFinancialResponsible = isFinancialResponsible,
            Gender = gender,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public string FullName => $"{FirstName} {LastName}";

    public void Update(
        string firstName,
        string lastName,
        string? email,
        string? phone,
        string? phoneSecondary,
        string? whatsApp,
        string? address,
        string? occupation,
        string? workplace,
        bool isFinancialResponsible)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email?.ToLowerInvariant();
        Phone = phone;
        PhoneSecondary = phoneSecondary;
        WhatsApp = whatsApp;
        Address = address;
        Occupation = occupation;
        Workplace = workplace;
        IsFinancialResponsible = isFinancialResponsible;
        UpdatedAt = DateTime.UtcNow;
    }

    public void LinkUser(Guid userId)
    {
        UserId = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UnlinkUser()
    {
        UserId = null;
        UpdatedAt = DateTime.UtcNow;
    }
}
