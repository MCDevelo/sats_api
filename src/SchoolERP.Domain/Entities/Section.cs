using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

public class Section : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid SchoolId { get; private set; }
    public Guid GradeLevelId { get; private set; }
    public Guid AcademicYearId { get; private set; }
    public string Name { get; private set; } = default!; // "A", "B", "C"
    public Shift Shift { get; private set; }
    public int Capacity { get; private set; }
    public Guid? HomeTeacherId { get; private set; }
    public string? Classroom { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation
    public Tenant Tenant { get; private set; } = default!;
    public School School { get; private set; } = default!;
    public GradeLevel GradeLevel { get; private set; } = default!;
    public AcademicYear AcademicYear { get; private set; } = default!;
    public Teacher? HomeTeacher { get; private set; }
    public ICollection<Enrollment> Enrollments { get; private set; } = [];
    public ICollection<TeacherAssignment> TeacherAssignments { get; private set; } = [];
    public ICollection<AttendanceRecord> AttendanceRecords { get; private set; } = [];

    private Section() { }

    public static Section Create(
        Guid tenantId,
        Guid schoolId,
        Guid gradeLevelId,
        Guid academicYearId,
        string name,
        Shift shift,
        int capacity = 35,
        string? classroom = null)
    {
        return new Section
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            SchoolId = schoolId,
            GradeLevelId = gradeLevelId,
            AcademicYearId = academicYearId,
            Name = name,
            Shift = shift,
            Capacity = capacity,
            Classroom = classroom,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, Shift shift, int capacity, string? classroom)
    {
        Name = name;
        Shift = shift;
        Capacity = capacity;
        Classroom = classroom;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignHomeTeacher(Guid teacherId)
    {
        HomeTeacherId = teacherId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveHomeTeacher()
    {
        HomeTeacherId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCapacity(int capacity)
    {
        Capacity = capacity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
