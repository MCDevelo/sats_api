using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

public class TeacherAssignment : BaseEntity
{
    public Guid TeacherId { get; private set; }
    public Guid SectionId { get; private set; }
    public Guid SubjectId { get; private set; }
    public Guid AcademicYearId { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation
    public Teacher Teacher { get; private set; } = default!;
    public Section Section { get; private set; } = default!;
    public Subject Subject { get; private set; } = default!;
    public AcademicYear AcademicYear { get; private set; } = default!;

    private TeacherAssignment() { }

    public static TeacherAssignment Create(
        Guid teacherId,
        Guid sectionId,
        Guid subjectId,
        Guid academicYearId)
    {
        return new TeacherAssignment
        {
            Id = Guid.NewGuid(),
            TeacherId = teacherId,
            SectionId = sectionId,
            SubjectId = subjectId,
            AcademicYearId = academicYearId,
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
