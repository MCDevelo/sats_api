using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Application.Auth.Commands.Login;
using SchoolERP.Application.Auth.Commands.Logout;
using SchoolERP.Application.Auth.Commands.RefreshToken;

namespace SchoolERP.Api.Controllers;

public class AuthController : BaseApiController
{
    /// <summary>
    /// Autenticar usuario con email/teléfono y contraseña.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(
            EmailOrPhone: request.EmailOrPhone,
            Password: request.Password,
            DeviceInfo: request.DeviceInfo,
            IpAddress: HttpContext.Connection.RemoteIpAddress?.ToString());

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Renovar access token usando el refresh token.
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(
            Token: request.Token,
            IpAddress: HttpContext.Connection.RemoteIpAddress?.ToString());

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Cerrar sesión revocando el refresh token.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new LogoutCommand(request.RefreshToken), cancellationToken);
        return HandleResult(result);
    }
}

public record LoginRequest(string EmailOrPhone, string Password, string? DeviceInfo = null);
public record RefreshRequest(string Token);
public record LogoutRequest(string RefreshToken);
