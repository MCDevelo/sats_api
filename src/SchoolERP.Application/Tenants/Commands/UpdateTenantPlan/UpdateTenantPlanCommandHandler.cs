using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Tenants.Commands.UpdateTenantPlan;

public class UpdateTenantPlanCommandHandler : IRequestHandler<UpdateTenantPlanCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;

    public UpdateTenantPlanCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<ErrorOr<Success>> Handle(UpdateTenantPlanCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _db.Tenants
            .FirstOrDefaultAsync(t => t.Id == request.TenantId, cancellationToken);

        if (tenant is null)
            return Error.NotFound("Tenant.NotFound", "Tenant no encontrado.");

        var validPlans = new[] { "trial", "starter", "professional", "enterprise" };
        if (!validPlans.Contains(request.Plan, StringComparer.OrdinalIgnoreCase))
            return Error.Validation("Tenant.InvalidPlan", $"Plan inválido. Opciones: {string.Join(", ", validPlans)}.");

        tenant.UpdatePlan(request.Plan.ToLower());
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}
