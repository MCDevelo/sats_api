using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Guardians.Commands.LinkGuardianToStudent;

public record LinkGuardianToStudentCommand(
    Guid StudentId,
    Guid GuardianId,
    string Relationship,
    bool IsPrimary = false,
    bool CanPickup = true,
    bool IsEmergencyContact = false,
    bool ReceivesNotifications = true,
    bool HasCustodyOrder = false,
    string? CustodyNotes = null) : IRequest<ErrorOr<StudentGuardianResult>>;

public record StudentGuardianResult(
    Guid Id,
    Guid StudentId,
    string StudentFullName,
    Guid GuardianId,
    string GuardianFullName,
    string? GuardianPhone,
    string? GuardianEmail,
    string? GuardianWhatsApp,
    string Relationship,
    bool IsPrimary,
    bool CanPickup,
    bool IsEmergencyContact,
    bool ReceivesNotifications,
    bool HasCustodyOrder,
    string? CustodyNotes);
