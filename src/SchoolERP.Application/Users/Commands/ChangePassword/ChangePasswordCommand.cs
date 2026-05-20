using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Users.Commands.ChangePassword;

/// <summary>
/// El usuario cambia su propia contraseña verificando la actual.
/// </summary>
public record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword) : IRequest<ErrorOr<Success>>;
