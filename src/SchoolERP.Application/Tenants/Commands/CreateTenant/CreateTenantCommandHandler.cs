using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Tenants.Commands.CreateTenant;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, ErrorOr<Guid>>
{
    private readonly IApplicationDbContext _db;

    public CreateTenantCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<ErrorOr<Guid>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var emailTaken = await _db.Tenants
            .AnyAsync(t => t.ContactEmail == request.ContactEmail, cancellationToken);

        if (emailTaken)
            return Error.Conflict("Tenant.EmailTaken", "Ya existe un tenant con ese email de contacto.");

        var tenant = Tenant.Create(
            request.Name,
            request.LegalName,
            request.ContactEmail,
            request.Rnc,
            request.ContactPhone);

        _db.Tenants.Add(tenant);
        await _db.SaveChangesAsync(cancellationToken);

        return tenant.Id;
    }
}
