using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Finance.Commands.ApplyDiscount;

public class ApplyDiscountCommandHandler : IRequestHandler<ApplyDiscountCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public ApplyDiscountCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(ApplyDiscountCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var invoice = await _db.Invoices
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId && i.TenantId == tenantId, cancellationToken);

        if (invoice is null)
            return Error.NotFound(description: "Factura no encontrada.");

        if (invoice.Status is InvoiceStatus.Paid or InvoiceStatus.Cancelled)
            return Error.Conflict(description: "No se puede aplicar descuento a una factura pagada o cancelada.");

        if (request.Discount >= invoice.Amount)
            return Error.Validation(description: "El descuento no puede ser mayor o igual al monto total de la factura.");

        // If there are existing payments, ensure discount doesn't reduce balance below zero
        if (invoice.AmountPaid > invoice.Amount - request.Discount)
            return Error.Validation(description: "El descuento no puede reducir el balance por debajo de los pagos ya realizados.");

        invoice.ApplyDiscount(request.Discount);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
