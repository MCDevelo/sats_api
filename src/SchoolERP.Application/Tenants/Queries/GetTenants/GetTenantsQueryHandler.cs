using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Models;

namespace SchoolERP.Application.Tenants.Queries.GetTenants;

public class GetTenantsQueryHandler : IRequestHandler<GetTenantsQuery, ErrorOr<PagedResult<TenantResult>>>
{
    private readonly IApplicationDbContext _db;

    public GetTenantsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<ErrorOr<PagedResult<TenantResult>>> Handle(GetTenantsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Tenants.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(t =>
                t.Name.ToLower().Contains(search) ||
                t.LegalName.ToLower().Contains(search) ||
                t.ContactEmail.ToLower().Contains(search));
        }

        if (request.IsActive.HasValue)
            query = query.Where(t => t.IsActive == request.IsActive.Value);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(t => t.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
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
            .ToListAsync(cancellationToken);

        return new PagedResult<TenantResult>(items, total, request.Page, request.PageSize);
    }
}
