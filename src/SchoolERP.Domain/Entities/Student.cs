using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

public class Student : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid SchoolId { get; private set; }
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string? SecondLastName { get; private set; }
    public string? NationalId { get; private set; }
    public string? Nse { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }
    public string? Nationality { get; private set; } = "Dominicana";
    public string? BloodType { get; private set; }
    public string? Allergies { get; private set; }
    public string? MedicalNotes { get; private set; }
    public string? HealthInsurance { get; private set; }
    public string? HealthInsuranceNumber { get; private set; }
    public bool HasSpecialNeeds { get; private set; }
    public string? Address { get; private set; }
    public string? Province { get; private set; }
    public string? Municipality { get; private set; }
    public string? Phone { get; private set; }
    public string? PhotoUrl { get; private set; }
    public string? StudentCode { get; private set; }
    public bool IsActive { get; private set; } = true;
    public Guid? UserId { get; private set; }

    // Navigation
    public Tenant Tenant { get; private set; } = default!;
    public School School { get; private set; } = default!;
    public User? User { get; private set; }
    public ICollection<StudentGuardian> StudentGuardians { get; private set; } = [];
    public ICollection<Enrollment> Enrollments { get; private set; } = [];
    public ICollection<AttendanceRecord> AttendanceRecords { get; private set; } = [];
    public ICollection<GradeEntry> GradeEntries { get; private set; } = [];
    public ICollection<Invoice> Invoices { get; private set; } = [];

    private Student() { }

    public static Student Create(
        Guid tenantId,
        Guid schoolId,
        string firstName,
        string lastName,
        DateTime dateOfBirth,
        Gender gender,
        string? secondLastName = null,
        string? nationalId = null,
        string? nse = null,
        string? studentCode = null,
        string? nationality = null,
        string? address = null,
        string? province = null,
        string? municipality = null,
        string? phone = null,
        string? bloodType = null,
        string? allergies = null,
        string? medicalNotes = null,
        string? healthInsurance = null,
        string? healthInsuranceNumber = null,
        bool hasSpecialNeeds = false)
    {
        return new Student
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            SchoolId = schoolId,
            FirstName = firstName,
            LastName = lastName,
            SecondLastName = secondLastName,
            NationalId = nationalId,
            Nse = nse,
            DateOfBirth = dateOfBirth,
            Gender = gender,
            Nationality = nationality ?? "Dominicana",
            Address = address,
            Province = province,
            Municipality = municipality,
            Phone = phone,
            BloodType = bloodType,
            Allergies = allergies,
            MedicalNotes = medicalNotes,
            HealthInsurance = healthInsurance,
            HealthInsuranceNumber = healthInsuranceNumber,
            HasSpecialNeeds = hasSpecialNeeds,
            StudentCode = studentCode,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public string FullName => string.IsNullOrEmpty(SecondLastName)
        ? $"{FirstName} {LastName}"
        : $"{FirstName} {LastName} {SecondLastName}";

    public int Age => (int)((DateTime.UtcNow - DateOfBirth).TotalDays / 365.25);

    public void Update(
        string firstName,
        string lastName,
        string? secondLastName,
        string? address,
        string? province,
        string? municipality,
        string? phone,
        string? medicalNotes,
        string? allergies,
        bool hasSpecialNeeds,
        string? healthInsurance,
        string? healthInsuranceNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        SecondLastName = secondLastName;
        Address = address;
        Province = province;
        Municipality = municipality;
        Phone = phone;
        MedicalNotes = medicalNotes;
        Allergies = allergies;
        HasSpecialNeeds = hasSpecialNeeds;
        HealthInsurance = healthInsurance;
        HealthInsuranceNumber = healthInsuranceNumber;
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
