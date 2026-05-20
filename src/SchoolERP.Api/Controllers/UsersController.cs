using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Application.Users.Commands.ActivateUser;
using SchoolERP.Application.Users.Commands.ChangePassword;
using SchoolERP.Application.Users.Commands.CreateUser;
using SchoolERP.Application.Users.Commands.DeactivateUser;
using SchoolERP.Application.Users.Commands.ResetUserPassword;
using SchoolERP.Application.Users.Commands.UnlockUser;
using SchoolERP.Application.Users.Commands.UpdateUser;
using SchoolERP.Application.Users.Queries.GetCurrentUser;
using SchoolERP.Application.Users.Queries.GetUser;
using SchoolERP.Application.Users.Queries.GetUsers;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Api.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    // ── Perfil propio ──────────────────────────────────────────────────────────

    /// <summary>Perfil del usuario autenticado.</summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetCurrentUserQuery(), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>El usuario cambia su propia contraseña.</summary>
    [HttpPatch("me/password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    // ── Administración de usuarios (Admin) ─────────────────────────────────────

    /// <summary>Lista paginada de usuarios del tenant. Filtros: search, role, isActive.</summary>
    [HttpGet]
    [Authorize(Roles = "Admin,SchoolAdmin,Director,Secretary")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] UserRole? role = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new GetUsersQuery(page, pageSize, search, role, isActive), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Detalle de un usuario.</summary>
    [HttpGet("{userId:guid}")]
    [Authorize(Roles = "Admin,SchoolAdmin,Director,Secretary")]
    public async Task<IActionResult> GetUser(Guid userId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetUserQuery(userId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Crear usuario (Admin asigna rol e credenciales).</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,SchoolAdmin")]
    public async Task<IActionResult> CreateUser(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Actualizar rol y contacto de un usuario.</summary>
    [HttpPut("{userId:guid}")]
    [Authorize(Roles = "Admin,SchoolAdmin")]
    public async Task<IActionResult> UpdateUser(Guid userId, UpdateUserCommand command, CancellationToken cancellationToken)
    {
        if (userId != command.UserId) return BadRequest();
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Desactivar usuario (no puede iniciar sesión).</summary>
    [HttpDelete("{userId:guid}")]
    [Authorize(Roles = "Admin,SchoolAdmin")]
    public async Task<IActionResult> DeactivateUser(Guid userId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeactivateUserCommand(userId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Reactivar usuario previamente desactivado.</summary>
    [HttpPatch("{userId:guid}/activate")]
    [Authorize(Roles = "Admin,SchoolAdmin")]
    public async Task<IActionResult> ActivateUser(Guid userId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new ActivateUserCommand(userId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Admin resetea la contraseña de un usuario.</summary>
    [HttpPatch("{userId:guid}/reset-password")]
    [Authorize(Roles = "Admin,SchoolAdmin")]
    public async Task<IActionResult> ResetPassword(Guid userId, ResetUserPasswordCommand command, CancellationToken cancellationToken)
    {
        if (userId != command.UserId) return BadRequest();
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Desbloquear usuario bloqueado por intentos fallidos de login.</summary>
    [HttpPatch("{userId:guid}/unlock")]
    [Authorize(Roles = "Admin,SchoolAdmin")]
    public async Task<IActionResult> UnlockUser(Guid userId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new UnlockUserCommand(userId), cancellationToken);
        return HandleResult(result);
    }
}
