using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Tenants.Commands.CreateTenant;

public record CreateTenantCommand(
    string Name,
    string LegalName,
    string ContactEmail,
    string? Rnc = null,
    string? ContactPhone = null) : IRequest<ErrorOr<Guid>>;
