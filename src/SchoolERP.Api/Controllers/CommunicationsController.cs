using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Application.Communications.Commands.CreateAnnouncement;
using SchoolERP.Application.Communications.Commands.MarkMessageRead;
using SchoolERP.Application.Communications.Commands.PublishAnnouncement;
using SchoolERP.Application.Communications.Commands.SendMessage;
using SchoolERP.Application.Communications.Queries.GetAnnouncementById;
using SchoolERP.Application.Communications.Queries.GetAnnouncements;
using SchoolERP.Application.Communications.Queries.GetInbox;
using SchoolERP.Application.Communications.Queries.GetSentMessages;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Api.Controllers;

[Authorize]
public class CommunicationsController : BaseApiController
{
    // ── Announcements ─────────────────────────────────────────────────────────

    /// <summary>
    /// Listar comunicados de una escuela con filtros opcionales.
    /// </summary>
    [HttpGet("announcements")]
    public async Task<IActionResult> GetAnnouncements(
        [FromQuery] Guid schoolId,
        [FromQuery] AnnouncementAudience? audience = null,
        [FromQuery] Guid? audienceId = null,
        [FromQuery] bool? isPublished = null,
        [FromQuery] AnnouncementPriority? priority = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAnnouncementsQuery
        {
            SchoolId = schoolId,
            Audience = audience,
            AudienceId = audienceId,
            IsPublished = isPublished,
            Priority = priority,
            Page = page,
            PageSize = pageSize
        };

        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtener un comunicado por ID.
    /// </summary>
    [HttpGet("announcements/{id:guid}")]
    public async Task<IActionResult> GetAnnouncement(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAnnouncementByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Crear un comunicado. Con PublishNow = true se publica inmediatamente.
    /// </summary>
    [HttpPost("announcements")]
    public async Task<IActionResult> CreateAnnouncement(
        [FromBody] CreateAnnouncementRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateAnnouncementCommand(
            SchoolId: request.SchoolId,
            Title: request.Title,
            Body: request.Body,
            Audience: request.Audience,
            Priority: request.Priority,
            AudienceId: request.AudienceId,
            ExpiresAt: request.ExpiresAt,
            PublishNow: request.PublishNow);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Publicar un comunicado en borrador.
    /// </summary>
    [HttpPost("announcements/{id:guid}/publish")]
    public async Task<IActionResult> PublishAnnouncement(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new PublishAnnouncementCommand(id), cancellationToken);
        return HandleResult(result);
    }

    // ── Messages ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Bandeja de entrada del usuario autenticado.
    /// </summary>
    [HttpGet("messages/inbox")]
    public async Task<IActionResult> GetInbox(
        [FromQuery] bool? onlyUnread = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new GetInboxQuery(onlyUnread, page, pageSize), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Mensajes enviados por el usuario autenticado.
    /// </summary>
    [HttpGet("messages/sent")]
    public async Task<IActionResult> GetSent(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new GetSentMessagesQuery(page, pageSize), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Enviar un mensaje directo a otro usuario del mismo tenant.
    /// ParentMessageId permite responder en un hilo existente.
    /// </summary>
    [HttpPost("messages")]
    public async Task<IActionResult> SendMessage(
        [FromBody] SendMessageRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SendMessageCommand(
            RecipientId: request.RecipientId,
            Subject: request.Subject,
            Body: request.Body,
            ParentMessageId: request.ParentMessageId);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Marcar un mensaje recibido como leído.
    /// </summary>
    [HttpPut("messages/{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new MarkMessageReadCommand(id), cancellationToken);
        return HandleResult(result);
    }
}

// Request bodies
public record CreateAnnouncementRequest(
    Guid SchoolId,
    string Title,
    string Body,
    AnnouncementAudience Audience,
    AnnouncementPriority Priority,
    Guid? AudienceId = null,
    DateTime? ExpiresAt = null,
    bool PublishNow = false);

public record SendMessageRequest(
    Guid RecipientId,
    string Subject,
    string Body,
    Guid? ParentMessageId = null);
