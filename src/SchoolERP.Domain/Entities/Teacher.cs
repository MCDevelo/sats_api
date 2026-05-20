using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

public class Teacher : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid SchoolId { get; private set; }
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string? NationalId { get; private set; }
    public string? MinerdCode { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public Gender? Gender { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public string? Address { get; private set; }
    public string? PhotoUrl { get; private set; }
    public string? TeacherCode { get; private set; }
    public string? AcademicTitle { get; private set; }
    public string? Specialization { get; private set; }
    public string? Qualifications { get; private set; }
    public ContractType ContractType { get; private set; }
    public DateTime HireDate { get; private set; }
    public DateTime? ContractEndDate { get; private set; }
    public int WorkingHoursPerWeek { get; private set; } = 40;
    public bool IsActive { get; private set; } = true;
    public Guid? UserId { get; private set; }

    // Navigation
    public Tenant Tenant { get; private set; } = default!;
    public School School { get; private set; } = default!;
    public User? User { get; private set; }
    public ICollection<TeacherAssignment> Assignments { get; private set; } = [];
    public ICollection<AttendanceRecord> AttendanceRecords { get; private set; } = [];

    private Teacher() { }

    public static Teacher Create(
        Guid tenantId,
        Guid schoolId,
        string firstName,
        string lastName,
        ContractType contractType,
        DateTime hireDate,
        string? email = null,
        string? phone = null,
        string? nationalId = null,
        string? minerdCode = null,
        string? teacherCode = null,
        string? academicTitle = null,
        string? specialization = null,
        string? qualifications = null,
        Gender? gender = null,
        DateTime? dateOfBirth = null,
        string? address = null,
        DateTime? contractEndDate = null,
        int workingHoursPerWeek = 40)
    {
        return new Teacher
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            SchoolId = schoolId,
            FirstName = firstName,
            LastName = lastName,
            ContractType = contractType,
            HireDate = hireDate,
            Email = email?.ToLowerInvariant(),
            Phone = phone,
            NationalId = nationalId,
            MinerdCode = minerdCode,
            TeacherCode = teacherCode,
            AcademicTitle = academicTitle,
            Specialization = specialization,
            Qualifications = qualifications,
            Gender = gender,
            DateOfBirth = dateOfBirth,
            Address = address,
            ContractEndDate = contractEndDate,
            WorkingHoursPerWeek = workingHoursPerWeek,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public string FullName => $"{FirstName} {LastName}";

    public void Update(
        string firstName,
        string lastName,
        string? email,
        string? phone,
        string? address,
        string? specialization,
        string? qualifications,
        string? academicTitle,
        int workingHoursPerWeek,
        DateTime? contractEndDate)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email?.ToLowerInvariant();
        Phone = phone;
        Address = address;
        Specialization = specialization;
        Qualifications = qualifications;
        AcademicTitle = academicTitle;
        WorkingHoursPerWeek = workingHoursPerWeek;
        ContractEndDate = contractEndDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPhotoUrl(string url)
    {
        PhotoUrl = url;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void LinkUser(Guid userId)
    {
        UserId = userId;
        UpdatedAt = DateTime.UtcNow;
    }
}
