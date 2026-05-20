using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

public class GradeEntry : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid SubjectId { get; private set; }
    public Guid AcademicPeriodId { get; private set; }
    public Guid EvaluationPlanId { get; private set; }
    public Guid? TeacherId { get; private set; }
    public decimal Score { get; private set; } // 0-100 scale
    public string? Comments { get; private set; }
    public bool IsPublished { get; private set; }

    // Navigation
    public Tenant Tenant { get; private set; } = default!;
    public Student Student { get; private set; } = default!;
    public Subject Subject { get; private set; } = default!;
    public AcademicPeriod AcademicPeriod { get; private set; } = default!;
    public EvaluationPlan EvaluationPlan { get; private set; } = default!;
    public Teacher? Teacher { get; private set; }

    private GradeEntry() { }

    public static GradeEntry Create(
        Guid tenantId,
        Guid studentId,
        Guid subjectId,
        Guid academicPeriodId,
        Guid evaluationPlanId,
        decimal score,
        Guid? teacherId = null,
        string? comments = null)
    {
        if (score < 0 || score > 100)
            throw new ArgumentException("La calificación debe estar entre 0 y 100.");

        return new GradeEntry
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            StudentId = studentId,
            SubjectId = subjectId,
            AcademicPeriodId = academicPeriodId,
            EvaluationPlanId = evaluationPlanId,
            Score = score,
            TeacherId = teacherId,
            Comments = comments,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(decimal score, string? comments)
    {
        if (score < 0 || score > 100)
            throw new ArgumentException("La calificación debe estar entre 0 y 100.");

        Score = score;
        Comments = comments;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Publish()
    {
        IsPublished = true;
        UpdatedAt = DateTime.UtcNow;
    }

    // Dominican Republic scale: 60+ is passing
    public bool IsPassing() => Score >= 60;
    public string GetLetterGrade() => Score switch
    {
        >= 90 => "A",
        >= 80 => "B",
        >= 70 => "C",
        >= 60 => "D",
        _ => "F"
    };
}
