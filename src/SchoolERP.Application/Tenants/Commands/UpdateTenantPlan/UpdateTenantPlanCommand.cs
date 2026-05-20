using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Tenants.Commands.UpdateTenantPlan;

public record UpdateTenantPlanCommand(Guid TenantId, string Plan) : IRequest<ErrorOr<Success>>;
