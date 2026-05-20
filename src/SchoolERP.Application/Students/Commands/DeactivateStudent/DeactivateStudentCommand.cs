using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Students.Commands.DeactivateStudent;

public record DeactivateStudentCommand(Guid StudentId) : IRequest<ErrorOr<Success>>;
