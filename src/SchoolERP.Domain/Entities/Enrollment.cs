using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

public class Enrollment : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid SectionId { get; private set; }
    public Guid AcademicYearId { get; private set; }
    public EnrollmentStatus Status { get; private set; }
    public DateTime EnrollmentDate { get; private set; }
    public DateTime? WithdrawalDate { get; private set; }
    public string? WithdrawalReason { get; private set; }
    public string? Notes { get; private set; }

    // Navigation
    public Tenant Tenant { get; private set; } = default!;
    public Student Student { get; private set; } = default!;
    public Section Section { get; private set; } = default!;
    public AcademicYear AcademicYear { get; private set; } = default!;

    private Enrollment() { }

    public static Enrollment Create(
        Guid tenantId,
        Guid studentId,
        Guid sectionId,
        Guid academicYearId,
        DateTime? enrollmentDate = null)
    {
        return new Enrollment
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            StudentId = studentId,
            SectionId = sectionId,
            AcademicYearId = academicYearId,
            Status = EnrollmentStatus.Active,
            EnrollmentDate = enrollmentDate ?? DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Withdraw(string reason)
    {
        Status = EnrollmentStatus.Withdrawn;
        WithdrawalDate = DateTime.UtcNow;
        WithdrawalReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Transfer()
    {
        Status = EnrollmentStatus.Transferred;
        WithdrawalDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Graduate()
    {
        Status = EnrollmentStatus.Graduated;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = EnrollmentStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsActive() => Status == EnrollmentStatus.Active;
}
