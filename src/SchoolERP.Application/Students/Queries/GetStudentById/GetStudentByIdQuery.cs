using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Students.Queries.GetStudentById;

public record GetStudentByIdQuery(Guid StudentId) : IRequest<ErrorOr<StudentDetailResult>>;

public record StudentDetailResult(
    Guid Id,
    Guid TenantId,
    Guid SchoolId,
    string SchoolName,
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
    IReadOnlyList<GuardianSummary> Guardians,
    EnrollmentSummary? CurrentEnrollment);

public record GuardianSummary(
    Guid GuardianId,
    string FullName,
    string Relationship,
    string? Phone,
    string? Email,
    bool IsPrimary,
    bool CanPickup,
    bool IsFinancialResponsible);

public record EnrollmentSummary(
    Guid EnrollmentId,
    string AcademicYear,
    string GradeLevel,
    string Section,
    string Shift,
    string Status,
    string? HomeroomTeacher);
