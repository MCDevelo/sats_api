using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.ValueObjects;
using SchoolERP.Infrastructure.Persistence;

namespace SchoolERP.Infrastructure.Services;

public class PlanService : IPlanService
{
    private readonly ApplicationDbContext _db;

    public PlanService(ApplicationDbContext db) => _db = db;

    public async Task<PlanLimits> GetLimitsAsync(Guid tenantId, CancellationToken ct = default)
    {
        var plan = await _db.Tenants
            .Where(t => t.Id == tenantId)
            .Select(t => t.Plan)
            .FirstOrDefaultAsync(ct);

        return PlanLimits.FromPlan(plan ?? string.Empty);
    }

    public async Task<bool> IsPlanActiveAsync(Guid tenantId, CancellationToken ct = default)
    {
        var tenant = await _db.Tenants
            .Where(t => t.Id == tenantId)
            .Select(t => new { t.IsActive, t.Plan, t.TrialEndsAt })
            .FirstOrDefaultAsync(ct);

        if (tenant is null || !tenant.IsActive)
            return false;

        if (tenant.Plan == "trial" && tenant.TrialEndsAt.HasValue && tenant.TrialEndsAt.Value < DateTime.UtcNow)
            return false;

        return true;
    }
}
