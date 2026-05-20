using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Teachers.Commands.DeactivateTeacher;

public record DeactivateTeacherCommand(Guid TeacherId) : IRequest<ErrorOr<Success>>;
