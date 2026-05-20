using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Tenants.Commands.UpdateTenant;

public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;

    public UpdateTenantCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<ErrorOr<Success>> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _db.Tenants
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (tenant is null)
            return Error.NotFound("Tenant.NotFound", "Tenant no encontrado.");

        var emailTaken = await _db.Tenants
            .AnyAsync(t => t.ContactEmail == request.ContactEmail && t.Id != request.Id, cancellationToken);

        if (emailTaken)
            return Error.Conflict("Tenant.EmailTaken", "Ya existe un tenant con ese email de contacto.");

        tenant.Update(
            request.Name,
            request.LegalName,
            request.ContactEmail,
            request.Rnc,
            request.ContactPhone,
            request.LogoUrl,
            request.PrimaryColor);

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}
