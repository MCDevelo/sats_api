using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Teachers.Commands.RemoveAssignment;

public record RemoveAssignmentCommand(Guid AssignmentId) : IRequest<ErrorOr<Success>>;
