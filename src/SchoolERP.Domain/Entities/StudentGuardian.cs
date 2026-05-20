using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

public class StudentGuardian : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid GuardianId { get; private set; }

    /// <summary>padre | madre | tutor | abuelo | tio | hermano | otro</summary>
    public string Relationship { get; private set; } = default!;

    public bool IsPrimary { get; private set; }
    public bool CanPickup { get; private set; }
    public bool IsEmergencyContact { get; private set; }
    public bool ReceivesNotifications { get; private set; }
    public bool HasCustodyOrder { get; private set; }
    public string? CustodyNotes { get; private set; }

    // Navigation
    public Student Student { get; private set; } = default!;
    public Guardian Guardian { get; private set; } = default!;

    private StudentGuardian() { }

    public static StudentGuardian Create(
        Guid tenantId,
        Guid studentId,
        Guid guardianId,
        string relationship,
        bool isPrimary = false,
        bool canPickup = true,
        bool isEmergencyContact = false,
        bool receivesNotifications = true,
        bool hasCustodyOrder = false,
        string? custodyNotes = null)
    {
        return new StudentGuardian
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            StudentId = studentId,
            GuardianId = guardianId,
            Relationship = relationship,
            IsPrimary = isPrimary,
            CanPickup = canPickup,
            IsEmergencyContact = isEmergencyContact,
            ReceivesNotifications = receivesNotifications,
            HasCustodyOrder = hasCustodyOrder,
            CustodyNotes = custodyNotes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string relationship,
        bool isPrimary,
        bool canPickup,
        bool isEmergencyContact,
        bool receivesNotifications,
        bool hasCustodyOrder,
        string? custodyNotes)
    {
        Relationship = relationship;
        IsPrimary = isPrimary;
        CanPickup = canPickup;
        IsEmergencyContact = isEmergencyContact;
        ReceivesNotifications = receivesNotifications;
        HasCustodyOrder = hasCustodyOrder;
        CustodyNotes = custodyNotes;
        UpdatedAt = DateTime.UtcNow;
    }
}
