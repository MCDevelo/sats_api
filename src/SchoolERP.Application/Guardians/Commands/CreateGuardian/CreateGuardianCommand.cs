using ErrorOr;
using MediatR;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Guardians.Commands.CreateGuardian;

public record CreateGuardianCommand(
    string FirstName,
    string LastName,
    string? NationalId = null,
    string? Email = null,
    string? Phone = null,
    string? PhoneSecondary = null,
    string? WhatsApp = null,
    string? Address = null,
    string? Occupation = null,
    string? Workplace = null,
    bool IsFinancialResponsible = false,
    Gender? Gender = null) : IRequest<ErrorOr<GuardianResult>>;

public record GuardianResult(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string? NationalId,
    string? Email,
    string? Phone,
    string? PhoneSecondary,
    string? WhatsApp,
    string? Address,
    string? Occupation,
    string? Workplace,
    bool IsFinancialResponsible,
    string? Gender,
    Guid? UserId,
    DateTime CreatedAt);
