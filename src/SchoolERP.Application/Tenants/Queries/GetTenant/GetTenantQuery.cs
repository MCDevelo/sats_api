using ErrorOr;
using MediatR;
using SchoolERP.Application.Tenants.Queries.GetTenants;

namespace SchoolERP.Application.Tenants.Queries.GetTenant;

public record GetTenantQuery(Guid TenantId) : IRequest<ErrorOr<TenantResult>>;
