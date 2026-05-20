using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Tenants.Commands.DeactivateTenant;

public class DeactivateTenantCommandHandler : IRequestHandler<DeactivateTenantCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;

    public DeactivateTenantCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<ErrorOr<Success>> Handle(DeactivateTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _db.Tenants
            .FirstOrDefaultAsync(t => t.Id == request.TenantId, cancellationToken);

        if (tenant is null)
            return Error.NotFound("Tenant.NotFound", "Tenant no encontrado.");

        if (!tenant.IsActive)
            return Error.Conflict("Tenant.AlreadyInactive", "El tenant ya está desactivado.");

        tenant.Deactivate();
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}
