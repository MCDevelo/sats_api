using SchoolERP.Application.Teachers.Commands.CreateTeacher;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Teachers;

public static class TeacherMappings
{
    public static TeacherResult ToResult(this Teacher t) => new(
        Id: t.Id,
        TenantId: t.TenantId,
        SchoolId: t.SchoolId,
        FirstName: t.FirstName,
        LastName: t.LastName,
        FullName: t.FullName,
        Email: t.Email,
        Phone: t.Phone,
        NationalId: t.NationalId,
        MinerdCode: t.MinerdCode,
        TeacherCode: t.TeacherCode,
        AcademicTitle: t.AcademicTitle,
        Specialization: t.Specialization,
        Qualifications: t.Qualifications,
        ContractType: t.ContractType.ToString(),
        HireDate: t.HireDate,
        ContractEndDate: t.ContractEndDate,
        WorkingHoursPerWeek: t.WorkingHoursPerWeek,
        Gender: t.Gender?.ToString(),
        DateOfBirth: t.DateOfBirth,
        Address: t.Address,
        PhotoUrl: t.PhotoUrl,
        IsActive: t.IsActive,
        CreatedAt: t.CreatedAt);
}
