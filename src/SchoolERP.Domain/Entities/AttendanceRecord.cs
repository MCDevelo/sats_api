using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

public class AttendanceRecord : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid SectionId { get; private set; }
    public Guid AcademicPeriodId { get; private set; }
    public Guid? TeacherId { get; private set; }
    public DateOnly Date { get; private set; }
    public AttendanceStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public TimeOnly? ArrivalTime { get; private set; }

    // Navigation
    public Tenant Tenant { get; private set; } = default!;
    public Student Student { get; private set; } = default!;
    public Section Section { get; private set; } = default!;
    public AcademicPeriod AcademicPeriod { get; private set; } = default!;
    public Teacher? Teacher { get; private set; }

    private AttendanceRecord() { }

    public static AttendanceRecord Create(
        Guid tenantId,
        Guid studentId,
        Guid sectionId,
        Guid academicPeriodId,
        DateOnly date,
        AttendanceStatus status,
        Guid? teacherId = null,
        string? notes = null,
        TimeOnly? arrivalTime = null)
    {
        return new AttendanceRecord
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            StudentId = studentId,
            SectionId = sectionId,
            AcademicPeriodId = academicPeriodId,
            Date = date,
            Status = status,
            TeacherId = teacherId,
            Notes = notes,
            ArrivalTime = arrivalTime,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(AttendanceStatus status, string? notes)
    {
        Status = status;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsAbsent() => Status == AttendanceStatus.Absent;
    public bool IsPresent() => Status == AttendanceStatus.Present || Status == AttendanceStatus.Late || Status == AttendanceStatus.HalfDay;
}
