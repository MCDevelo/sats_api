using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Users.Commands.ActivateUser;

public record ActivateUserCommand(Guid UserId) : IRequest<ErrorOr<Success>>;
