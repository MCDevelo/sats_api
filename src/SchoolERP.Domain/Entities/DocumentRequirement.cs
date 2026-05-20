using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

public class DocumentRequirement : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid SchoolId { get; private set; }
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public bool IsRequired { get; private set; } = true;
    public string AcceptedFileTypes { get; private set; } = "pdf,jpg,jpeg,png";
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation
    public School School { get; private set; } = default!;
    public ICollection<StudentDocument> StudentDocuments { get; private set; } = [];

    private DocumentRequirement() { }

    public static DocumentRequirement Create(Guid tenantId, Guid schoolId, string name,
        bool isRequired, string? description = null, string? acceptedFileTypes = null, int displayOrder = 0)
    {
        return new DocumentRequirement
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            SchoolId = schoolId,
            Name = name,
            IsRequired = isRequired,
            Description = description,
            AcceptedFileTypes = acceptedFileTypes ?? "pdf,jpg,jpeg,png",
            DisplayOrder = displayOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, bool isRequired, string? description, string? acceptedFileTypes, int displayOrder)
    {
        Name = name;
        IsRequired = isRequired;
        Description = description;
        if (acceptedFileTypes is not null) AcceptedFileTypes = acceptedFileTypes;
        DisplayOrder = displayOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }
}
