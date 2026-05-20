using ErrorOr;
using MediatR;
using SchoolERP.Application.Students.Commands.CreateStudent;

namespace SchoolERP.Application.Students.Commands.UpdateStudent;

public record UpdateStudentCommand(
    Guid StudentId,
    string FirstName,
    string LastName,
    string? SecondLastName,
    string? Address,
    string? Province,
    string? Municipality,
    string? Phone,
    string? Allergies,
    string? MedicalNotes,
    bool HasSpecialNeeds,
    string? HealthInsurance,
    string? HealthInsuranceNumber) : IRequest<ErrorOr<StudentResult>>;
