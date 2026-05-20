using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Guardians.Commands.RemoveStudentGuardian;

public record RemoveStudentGuardianCommand(Guid StudentGuardianId) : IRequest<ErrorOr<Success>>;
