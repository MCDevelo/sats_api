using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

public class GradeLevel : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid SchoolId { get; private set; }
    public string Name { get; private set; } = default!; // "1ro Primaria", "2do Primaria", etc.
    public int Order { get; private set; }
    public EducationLevel EducationLevel { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation
    public Tenant Tenant { get; private set; } = default!;
    public School School { get; private set; } = default!;
    public ICollection<Section> Sections { get; private set; } = [];
    public ICollection<Subject> Subjects { get; private set; } = [];

    private GradeLevel() { }

    public static GradeLevel Create(
        Guid tenantId,
        Guid schoolId,
        string name,
        int order,
        EducationLevel educationLevel)
    {
        return new GradeLevel
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            SchoolId = schoolId,
            Name = name,
            Order = order,
            EducationLevel = educationLevel,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
