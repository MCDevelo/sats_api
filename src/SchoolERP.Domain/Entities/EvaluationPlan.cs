using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

public class EvaluationPlan : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid SubjectId { get; private set; }
    public Guid AcademicPeriodId { get; private set; }
    public string Name { get; private set; } = default!; // "Prueba 1", "Trabajo Práctico", "Examen Final"
    public string? Description { get; private set; }
    public decimal Weight { get; private set; } // Percentage 0-100
    public DateTime? DueDate { get; private set; }
    public bool IsPublished { get; private set; }

    // Navigation
    public Tenant Tenant { get; private set; } = default!;
    public Subject Subject { get; private set; } = default!;
    public AcademicPeriod AcademicPeriod { get; private set; } = default!;
    public ICollection<GradeEntry> GradeEntries { get; private set; } = [];

    private EvaluationPlan() { }

    public static EvaluationPlan Create(
        Guid tenantId,
        Guid subjectId,
        Guid academicPeriodId,
        string name,
        decimal weight,
        string? description = null,
        DateTime? dueDate = null)
    {
        if (weight <= 0 || weight > 100)
            throw new ArgumentException("El peso debe estar entre 0 y 100.");

        return new EvaluationPlan
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            SubjectId = subjectId,
            AcademicPeriodId = academicPeriodId,
            Name = name,
            Weight = weight,
            Description = description,
            DueDate = dueDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Publish()
    {
        IsPublished = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name, decimal weight, string? description, DateTime? dueDate)
    {
        Name = name;
        Weight = weight;
        Description = description;
        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;
    }
}
