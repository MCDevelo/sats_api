using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

public class AcademicPeriod : BaseEntity
{
    public Guid AcademicYearId { get; private set; }
    public string Name { get; private set; } = default!; // "1er Trimestre", "2do Trimestre", "3er Trimestre"
    public int PeriodNumber { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsActive { get; private set; }
    public bool GradesPublished { get; private set; }

    // Navigation
    public AcademicYear AcademicYear { get; private set; } = default!;
    public ICollection<GradeEntry> GradeEntries { get; private set; } = [];
    public ICollection<AttendanceRecord> AttendanceRecords { get; private set; } = [];

    private AcademicPeriod() { }

    public static AcademicPeriod Create(
        Guid academicYearId,
        string name,
        int periodNumber,
        DateTime startDate,
        DateTime endDate)
    {
        return new AcademicPeriod
        {
            Id = Guid.NewGuid(),
            AcademicYearId = academicYearId,
            Name = name,
            PeriodNumber = periodNumber,
            StartDate = startDate,
            EndDate = endDate,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void PublishGrades()
    {
        GradesPublished = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
