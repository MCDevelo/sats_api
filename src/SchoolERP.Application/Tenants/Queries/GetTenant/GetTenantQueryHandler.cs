using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Tenants.Queries.GetTenants;

namespace SchoolERP.Application.Tenants.Queries.GetTenant;

public class GetTenantQueryHandler : IRequestHandler<GetTenantQuery, ErrorOr<TenantResult>>
{
    private readonly IApplicationDbContext _db;

    public GetTenantQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<ErrorOr<TenantResult>> Handle(GetTenantQuery request, CancellationToken cancellationToken)
    {
        var tenant = await _db.Tenants
            .Where(t => t.Id == request.TenantId)
            .Select(t => new TenantResult(
                t.Id,
                t.Name,
                t.LegalName,
                t.Rnc,
                t.ContactEmail,
                t.ContactPhone,
                t.Plan,
                t.IsActive,
                t.TrialEndsAt,
                t.ContractStart,
                t.ContractEnd,
                t.OnboardingCompleted,
                t.OnboardingStep,
                t.Schools.Count,
                t.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        if (tenant is null)
            return Error.NotFound("Tenant.NotFound", "Tenant no encontrado.");

        return tenant;
    }
}
