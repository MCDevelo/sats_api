using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Guardians.Commands.LinkUserToGuardian;

/// <summary>
/// Asocia una cuenta de usuario del portal de padres a un encargado.
/// Permite al encargado iniciar sesión y ver información de sus estudiantes.
/// </summary>
public record LinkUserToGuardianCommand(
    Guid GuardianId,
    Guid UserId) : IRequest<ErrorOr<Success>>;
