using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Tenants.Commands.UpdateTenant;

public record UpdateTenantCommand(
    Guid Id,
    string Name,
    string LegalName,
    string ContactEmail,
    string? Rnc = null,
    string? ContactPhone = null,
    string? LogoUrl = null,
    string? PrimaryColor = null) : IRequest<ErrorOr<Success>>;
