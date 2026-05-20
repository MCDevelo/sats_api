using SchoolERP.Application.Students.Commands.CreateStudent;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Students;

public static class StudentMappings
{
    public static StudentResult ToResult(this Student s) => new(
        Id: s.Id,
        TenantId: s.TenantId,
        SchoolId: s.SchoolId,
        FirstName: s.FirstName,
        LastName: s.LastName,
        SecondLastName: s.SecondLastName,
        FullName: s.FullName,
        DateOfBirth: s.DateOfBirth,
        Age: s.Age,
        Gender: s.Gender.ToString(),
        NationalId: s.NationalId,
        Nse: s.Nse,
        StudentCode: s.StudentCode,
        Address: s.Address,
        Province: s.Province,
        Municipality: s.Municipality,
        Phone: s.Phone,
        BloodType: s.BloodType,
        Allergies: s.Allergies,
        MedicalNotes: s.MedicalNotes,
        HealthInsurance: s.HealthInsurance,
        HealthInsuranceNumber: s.HealthInsuranceNumber,
        HasSpecialNeeds: s.HasSpecialNeeds,
        Nationality: s.Nationality,
        PhotoUrl: s.PhotoUrl,
        IsActive: s.IsActive,
        CreatedAt: s.CreatedAt);
}
