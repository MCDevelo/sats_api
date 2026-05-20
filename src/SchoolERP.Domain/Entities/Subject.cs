using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

public class Subject : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid SchoolId { get; private set; }
    public Guid GradeLevelId { get; private set; }
    public string Name { get; private set; } = default!;
    public string? Code { get; private set; }
    public string? Description { get; private set; }
    public int WeeklyHours { get; private set; }
    public bool IsRequired { get; private set; } = true;
    public bool IsActive { get; private set; } = true;

    // Navigation
    public Tenant Tenant { get; private set; } = default!;
    public School School { get; private set; } = default!;
    public GradeLevel GradeLevel { get; private set; } = default!;
    public ICollection<TeacherAssignment> TeacherAssignments { get; private set; } = [];
    public ICollection<GradeEntry> GradeEntries { get; private set; } = [];
    public ICollection<EvaluationPlan> EvaluationPlans { get; private set; } = [];

    private Subject() { }

    public static Subject Create(
        Guid tenantId,
        Guid schoolId,
        Guid gradeLevelId,
        string name,
        int weeklyHours,
        string? code = null,
        bool isRequired = true)
    {
        return new Subject
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            SchoolId = schoolId,
            GradeLevelId = gradeLevelId,
            Name = name,
            WeeklyHours = weeklyHours,
            Code = code,
            IsRequired = isRequired,
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
