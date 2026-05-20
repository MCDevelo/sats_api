using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Users.Commands.DeactivateUser;

public record DeactivateUserCommand(Guid UserId) : IRequest<ErrorOr<Success>>;
