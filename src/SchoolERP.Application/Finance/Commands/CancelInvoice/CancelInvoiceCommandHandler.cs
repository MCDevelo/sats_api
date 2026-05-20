using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Finance.Commands.CancelInvoice;

public class CancelInvoiceCommandHandler : IRequestHandler<CancelInvoiceCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CancelInvoiceCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(CancelInvoiceCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var invoice = await _db.Invoices
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId && i.TenantId == tenantId, cancellationToken);

        if (invoice is null)
            return Error.NotFound(description: "Factura no encontrada.");

        if (invoice.Status == InvoiceStatus.Cancelled)
            return Error.Conflict(description: "La factura ya está cancelada.");

        if (invoice.AmountPaid > 0)
            return Error.Conflict(description: "No se puede cancelar una factura con pagos registrados. Anule los pagos primero.");

        invoice.Cancel(request.Reason);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
