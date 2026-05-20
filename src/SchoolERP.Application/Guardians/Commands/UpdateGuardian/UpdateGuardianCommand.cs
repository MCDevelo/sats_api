using ErrorOr;
using MediatR;
using SchoolERP.Application.Guardians.Commands.CreateGuardian;

namespace SchoolERP.Application.Guardians.Commands.UpdateGuardian;

public record UpdateGuardianCommand(
    Guid GuardianId,
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    string? PhoneSecondary,
    string? WhatsApp,
    string? Address,
    string? Occupation,
    string? Workplace,
    bool IsFinancialResponsible) : IRequest<ErrorOr<GuardianResult>>;
