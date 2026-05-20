using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Application.Tenants.Commands.CreateTenant;
using SchoolERP.Application.Tenants.Commands.DeactivateTenant;
using SchoolERP.Application.Tenants.Commands.UpdateTenant;
using SchoolERP.Application.Tenants.Commands.UpdateTenantPlan;
using SchoolERP.Application.Tenants.Queries.GetTenant;
using SchoolERP.Application.Tenants.Queries.GetTenants;

namespace SchoolERP.Api.Controllers;

/// <summary>
/// Gestión de tenants (Solo PlatformAdmin).
/// </summary>
[Authorize(Roles = "PlatformAdmin")]
public class TenantsController : BaseApiController
{
    /// <summary>Lista paginada de todos los tenants.</summary>
    [HttpGet]
    public async Task<IActionResult> GetTenants(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new GetTenantsQuery(page, pageSize, search, isActive), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Detalle de un tenant específico.</summary>
    [HttpGet("{tenantId:guid}")]
    public async Task<IActionResult> GetTenant(Guid tenantId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetTenantQuery(tenantId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Crear nuevo tenant.</summary>
    [HttpPost]
    public async Task<IActionResult> CreateTenant(CreateTenantCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Actualizar datos del tenant.</summary>
    [HttpPut("{tenantId:guid}")]
    public async Task<IActionResult> UpdateTenant(Guid tenantId, UpdateTenantCommand command, CancellationToken cancellationToken)
    {
        if (tenantId != command.Id) return BadRequest();
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Cambiar plan de suscripción del tenant.</summary>
    [HttpPatch("{tenantId:guid}/plan")]
    public async Task<IActionResult> UpdatePlan(Guid tenantId, [FromBody] string plan, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new UpdateTenantPlanCommand(tenantId, plan), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Desactivar tenant.</summary>
    [HttpDelete("{tenantId:guid}")]
    public async Task<IActionResult> DeactivateTenant(Guid tenantId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeactivateTenantCommand(tenantId), cancellationToken);
        return HandleResult(result);
    }
}
