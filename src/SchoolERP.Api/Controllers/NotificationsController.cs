using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Application.Notifications.Commands.MarkAllAsRead;
using SchoolERP.Application.Notifications.Commands.MarkAsRead;
using SchoolERP.Application.Notifications.Queries.GetUnreadCount;
using SchoolERP.Application.Notifications.Queries.GetUserNotifications;

namespace SchoolERP.Api.Controllers;

[Authorize]
public class NotificationsController : BaseApiController
{
    /// <summary>
    /// Obtener notificaciones InApp del usuario autenticado (paginadas).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetNotifications(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? onlyUnread = null,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(
            new GetUserNotificationsQuery(page, pageSize, onlyUnread),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Número de notificaciones InApp no leídas del usuario autenticado.
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetUnreadCountQuery(), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Marcar una notificación específica como leída.
    /// </summary>
    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new MarkAsReadCommand(id), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Marcar todas las notificaciones no leídas del usuario como leídas.
    /// Devuelve el número de notificaciones actualizadas.
    /// </summary>
    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new MarkAllAsReadCommand(), cancellationToken);
        return HandleResult(result);
    }
}
