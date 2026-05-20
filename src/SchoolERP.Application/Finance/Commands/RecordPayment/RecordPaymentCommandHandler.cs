using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Finance.Commands.RecordPayment;

public class RecordPaymentCommandHandler : IRequestHandler<RecordPaymentCommand, ErrorOr<PaymentResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public RecordPaymentCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<PaymentResult>> Handle(RecordPaymentCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var invoice = await _db.Invoices
            .Include(i => i.Student)
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId && i.TenantId == tenantId, cancellationToken);

        if (invoice is null)
            return Error.NotFound(description: "Factura no encontrada.");

        if (invoice.Status == InvoiceStatus.Cancelled)
            return Error.Conflict(description: "No se puede registrar un pago en una factura cancelada.");

        if (invoice.Status == InvoiceStatus.Paid)
            return Error.Conflict(description: "La factura ya está completamente pagada.");

        if (request.Amount > invoice.Balance)
            return Error.Validation(description: $"El pago ({request.Amount:C}) excede el balance pendiente ({invoice.Balance:C}).");

        var payment = Payment.Create(
            tenantId: tenantId,
            invoiceId: invoice.Id,
            amount: request.Amount,
            method: request.Method,
            paidAt: request.PaidAt,
            reference: request.Reference,
            receivedBy: _currentUser.UserId,
            notes: request.Notes);

        _db.Payments.Add(payment);
        invoice.ApplyPayment(request.Amount);

        await _db.SaveChangesAsync(cancellationToken);

        return new PaymentResult(
            PaymentId: payment.Id,
            InvoiceId: invoice.Id,
            InvoiceNumber: invoice.InvoiceNumber,
            StudentFullName: invoice.Student.FullName,
            AmountPaid: payment.Amount,
            Method: payment.Method.ToString(),
            Reference: payment.Reference,
            PaidAt: payment.PaidAt,
            RemainingBalance: invoice.Balance,
            InvoiceStatus: invoice.Status.ToString());
    }
}
