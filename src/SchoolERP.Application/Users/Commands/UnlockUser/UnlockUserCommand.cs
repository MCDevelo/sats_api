using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Users.Commands.UnlockUser;

public record UnlockUserCommand(Guid UserId) : IRequest<ErrorOr<Success>>;
