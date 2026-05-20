using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Guardians.Queries.GetGuardianById;

public record GetGuardianByIdQuery(Guid GuardianId) : IRequest<ErrorOr<GuardianDetailResult>>;

public record GuardianDetailResult(
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
    DateTime CreatedAt,
    IReadOnlyList<GuardianStudentLink> Students);

public record GuardianStudentLink(
    Guid StudentGuardianId,
    Guid StudentId,
    string StudentFullName,
    string? StudentCode,
    string Relationship,
    bool IsPrimary,
    bool CanPickup,
    bool IsEmergencyContact,
    bool ReceivesNotifications,
    bool HasCustodyOrder);
