using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

public class AcademicYear : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid SchoolId { get; private set; }
    public string Name { get; private set; } = default!; // e.g. "2024-2025"
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsCurrent { get; private set; }

    // Navigation
    public Tenant Tenant { get; private set; } = default!;
    public School School { get; private set; } = default!;
    public ICollection<AcademicPeriod> Periods { get; private set; } = [];
    public ICollection<Enrollment> Enrollments { get; private set; } = [];

    private AcademicYear() { }

    public static AcademicYear Create(
        Guid tenantId,
        Guid schoolId,
        string name,
        DateTime startDate,
        DateTime endDate)
    {
        if (endDate <= startDate)
            throw new ArgumentException("La fecha de fin debe ser posterior a la fecha de inicio.");

        return new AcademicYear
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            SchoolId = schoolId,
            Name = name,
            StartDate = startDate,
            EndDate = endDate,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void SetAsCurrent()
    {
        IsCurrent = true;
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        IsCurrent = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
