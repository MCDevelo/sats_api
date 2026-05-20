using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Application.AuditLogs.Queries.GetAuditLogs;

namespace SchoolERP.Api.Controllers;

[Authorize(Roles = "PlatformAdmin,TenantAdmin,Director")]
public class AuditLogsController : BaseApiController
{
    /// <summary>
    /// Lista paginada del registro de auditoría del tenant.
    /// Filtros opcionales: entidad, acción (CREATE/UPDATE/DELETE), usuario, rango de fechas.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? entityName = null,
        [FromQuery] string? action = null,
        [FromQuery] string? entityId = null,
        [FromQuery] Guid? userId = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(
            new GetAuditLogsQuery(
                page, pageSize,
                entityName, action, entityId,
                userId, dateFrom, dateTo),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Historial de cambios de una entidad específica (ej: historial completo de un estudiante).
    /// </summary>
    [HttpGet("{entityName}/{entityId}")]
    public async Task<IActionResult> GetEntityHistory(
        string entityName,
        string entityId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(
            new GetAuditLogsQuery(
                Page: page,
                PageSize: pageSize,
                EntityName: entityName,
                EntityId: entityId),
            cancellationToken);

        return HandleResult(result);
    }
}
