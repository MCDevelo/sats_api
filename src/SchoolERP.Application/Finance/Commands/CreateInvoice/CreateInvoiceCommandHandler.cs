using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Finance.Commands.CreateInvoice;

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, ErrorOr<InvoiceResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateInvoiceCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<InvoiceResult>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var student = await _db.Students
            .FirstOrDefaultAsync(s => s.Id == request.StudentId && s.TenantId == tenantId && s.IsActive, cancellationToken);

        if (student is null)
            return Error.NotFound(description: "Estudiante no encontrado.");

        var academicYear = await _db.AcademicYears
            .FirstOrDefaultAsync(ay => ay.Id == request.AcademicYearId && ay.TenantId == tenantId, cancellationToken);

        if (academicYear is null)
            return Error.NotFound(description: "Año académico no encontrado.");

        // Prevent duplicate invoice for same student/month/year
        var duplicate = await _db.Invoices
            .AnyAsync(i =>
                i.StudentId == request.StudentId &&
                i.Month == request.Month &&
                i.Year == request.Year &&
                i.AcademicYearId == request.AcademicYearId, cancellationToken);

        if (duplicate)
            return Error.Conflict(description: $"Ya existe una factura para este estudiante en {request.Month}/{request.Year}.");

        var invoiceNumber = await GenerateInvoiceNumberAsync(tenantId, cancellationToken);

        var invoice = Invoice.Create(
            tenantId: tenantId,
            studentId: request.StudentId,
            academicYearId: request.AcademicYearId,
            invoiceNumber: invoiceNumber,
            description: request.Description,
            amount: request.Amount,
            dueDate: request.DueDate,
            month: request.Month,
            year: request.Year,
            discount: request.Discount,
            ncf: request.Ncf);

        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync(cancellationToken);

        return invoice.ToResult(student.FullName);
    }

    private async Task<string> GenerateInvoiceNumberAsync(Guid tenantId, CancellationToken ct)
    {
        var count = await _db.Invoices.CountAsync(i => i.TenantId == tenantId, ct);
        return $"INV-{DateTime.UtcNow:yyyy}-{(count + 1):D6}";
    }
}
