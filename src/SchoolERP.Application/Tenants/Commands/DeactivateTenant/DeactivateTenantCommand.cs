using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Tenants.Commands.DeactivateTenant;

public record DeactivateTenantCommand(Guid TenantId) : IRequest<ErrorOr<Success>>;
