using ErrorOr;
using MediatR;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Students.Commands.CreateStudent;

public record CreateStudentCommand(
    Guid SchoolId,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    Gender Gender,
    string? SecondLastName = null,
    string? NationalId = null,
    string? Nse = null,
    string? StudentCode = null,
    string? Nationality = null,
    string? Address = null,
    string? Province = null,
    string? Municipality = null,
    string? Phone = null,
    string? BloodType = null,
    string? Allergies = null,
    string? MedicalNotes = null,
    string? HealthInsurance = null,
    string? HealthInsuranceNumber = null,
    bool HasSpecialNeeds = false) : IRequest<ErrorOr<StudentResult>>;

public record GuardianPrimary(string Name, string? Phone);

public record StudentResult(
    Guid Id,
    Guid TenantId,
    Guid SchoolId,
    string FirstName,
    string LastName,
    string? SecondLastName,
    string FullName,
    DateTime DateOfBirth,
    int Age,
    string Gender,
    string? NationalId,
    string? Nse,
    string? StudentCode,
    string? Address,
    string? Province,
    string? Municipality,
    string? Phone,
    string? BloodType,
    string? Allergies,
    string? MedicalNotes,
    string? HealthInsurance,
    string? HealthInsuranceNumber,
    bool HasSpecialNeeds,
    string? Nationality,
    string? PhotoUrl,
    bool IsActive,
    DateTime CreatedAt,
    GuardianPrimary? GuardianPrimary = null);
