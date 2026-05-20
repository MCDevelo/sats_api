using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Auth.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest<ErrorOr<Success>>;
