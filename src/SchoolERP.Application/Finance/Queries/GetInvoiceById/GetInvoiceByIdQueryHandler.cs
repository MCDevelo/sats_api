using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Finance.Queries.GetInvoiceById;

public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, ErrorOr<InvoiceDetailResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetInvoiceByIdQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<InvoiceDetailResult>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var invoice = await _db.Invoices
            .AsNoTracking()
            .Include(i => i.Student)
            .Include(i => i.AcademicYear)
            .Include(i => i.Payments.OrderByDescending(p => p.PaidAt))
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId && i.TenantId == tenantId, cancellationToken);

        if (invoice is null)
            return Error.NotFound(description: "Factura no encontrada.");

        var payments = invoice.Payments.Select(p => new PaymentSummary(
            PaymentId: p.Id,
            Amount: p.Amount,
            Method: p.Method.ToString(),
            Reference: p.Reference,
            PaidAt: p.PaidAt,
            Notes: p.Notes)).ToList();

        return new InvoiceDetailResult(
            Id: invoice.Id,
            InvoiceNumber: invoice.InvoiceNumber,
            Ncf: invoice.Ncf,
            StudentId: invoice.Student.Id,
            StudentFullName: invoice.Student.FullName,
            StudentCode: invoice.Student.StudentCode,
            AcademicYear: invoice.AcademicYear.Name,
            Description: invoice.Description,
            Amount: invoice.Amount,
            Discount: invoice.Discount,
            AmountPaid: invoice.AmountPaid,
            Balance: invoice.Balance,
            Status: invoice.Status.ToString(),
            DueDate: invoice.DueDate,
            IsOverdue: invoice.IsOverdue,
            Month: invoice.Month,
            Year: invoice.Year,
            CreatedAt: invoice.CreatedAt,
            Payments: payments);
    }
}
