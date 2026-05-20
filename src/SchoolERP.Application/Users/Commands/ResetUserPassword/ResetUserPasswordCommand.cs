using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Users.Commands.ResetUserPassword;

/// <summary>
/// El Admin establece una nueva contraseña para otro usuario (reset administrativo).
/// </summary>
public record ResetUserPasswordCommand(
    Guid UserId,
    string NewPassword) : IRequest<ErrorOr<Success>>;
