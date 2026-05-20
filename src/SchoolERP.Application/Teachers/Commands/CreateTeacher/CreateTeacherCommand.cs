using ErrorOr;
using MediatR;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Teachers.Commands.CreateTeacher;

public record CreateTeacherCommand(
    Guid SchoolId,
    string FirstName,
    string LastName,
    ContractType ContractType,
    DateTime HireDate,
    string? Email = null,
    string? Phone = null,
    string? NationalId = null,
    string? MinerdCode = null,
    string? TeacherCode = null,
    string? AcademicTitle = null,
    string? Specialization = null,
    string? Qualifications = null,
    Gender? Gender = null,
    DateTime? DateOfBirth = null,
    string? Address = null,
    DateTime? ContractEndDate = null,
    int WorkingHoursPerWeek = 40) : IRequest<ErrorOr<TeacherResult>>;

public record TeacherResult(
    Guid Id,
    Guid TenantId,
    Guid SchoolId,
    string FirstName,
    string LastName,
    string FullName,
    string? Email,
    string? Phone,
    string? NationalId,
    string? MinerdCode,
    string? TeacherCode,
    string? AcademicTitle,
    string? Specialization,
    string? Qualifications,
    string ContractType,
    DateTime HireDate,
    DateTime? ContractEndDate,
    int WorkingHoursPerWeek,
    string? Gender,
    DateTime? DateOfBirth,
    string? Address,
    string? PhotoUrl,
    bool IsActive,
    DateTime CreatedAt);
