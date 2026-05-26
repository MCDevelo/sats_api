using SchoolERP.Domain.ValueObjects;

namespace SchoolERP.Application.Common.Interfaces;

public interface IPlanService
{
    Task<PlanLimits> GetLimitsAsync(Guid tenantId, CancellationToken ct = default);
    Task<bool> IsPlanActiveAsync(Guid tenantId, CancellationToken ct = default);
}
