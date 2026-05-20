using ErrorOr;
using MediatR;
using SchoolERP.Application.Guardians.Commands.LinkGuardianToStudent;

namespace SchoolERP.Application.Guardians.Commands.UpdateStudentGuardian;

public record UpdateStudentGuardianCommand(
    Guid StudentGuardianId,
    string Relationship,
    bool IsPrimary,
    bool CanPickup,
    bool IsEmergencyContact,
    bool ReceivesNotifications,
    bool HasCustodyOrder,
    string? CustodyNotes = null) : IRequest<ErrorOr<StudentGuardianResult>>;
