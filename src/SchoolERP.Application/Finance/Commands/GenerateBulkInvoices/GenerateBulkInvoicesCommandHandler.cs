using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Finance.Commands.GenerateBulkInvoices;

public class GenerateBulkInvoicesCommandHandler : IRequestHandler<GenerateBulkInvoicesCommand, ErrorOr<BulkInvoiceResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GenerateBulkInvoicesCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<BulkInvoiceResult>> Handle(GenerateBulkInvoicesCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var schoolExists = await _db.Schools
            .AnyAsync(s => s.Id == request.SchoolId && s.TenantId == tenantId && s.IsActive, cancellationToken);

        if (!schoolExists)
            return Error.NotFound(description: "Escuela no encontrada.");

        // All active students enrolled in this academic year at this school
        var eligibleStudentIds = await _db.Enrollments
            .AsNoTracking()
            .Where(e =>
                e.AcademicYearId == request.AcademicYearId &&
                e.Status == EnrollmentStatus.Active &&
                e.TenantId == tenantId)
            .Include(e => e.Student)
            .Where(e => e.Student.SchoolId == request.SchoolId && e.Student.IsActive)
            .Select(e => e.StudentId)
            .Distinct()
            .ToListAsync(cancellationToken);

        // Students that already have an invoice for this month/year — skip them
        var alreadyInvoiced = await _db.Invoices
            .AsNoTracking()
            .Where(i =>
                i.TenantId == tenantId &&
                i.AcademicYearId == request.AcademicYearId &&
                i.Month == request.Month &&
                i.Year == request.Year &&
                eligibleStudentIds.Contains(i.StudentId))
            .Select(i => i.StudentId)
            .ToListAsync(cancellationToken);

        var toCreate = eligibleStudentIds.Except(alreadyInvoiced).ToList();

        if (toCreate.Count == 0)
            return new BulkInvoiceResult(0, eligibleStudentIds.Count, 0,
                $"{request.Month:D2}/{request.Year}");

        var baseCount = await _db.Invoices.CountAsync(i => i.TenantId == tenantId, cancellationToken);
        var newInvoices = new List<Invoice>();

        foreach (var (studentId, idx) in toCreate.Select((id, i) => (id, i)))
        {
            var invoiceNumber = $"INV-{request.Year}-{(baseCount + idx + 1):D6}";

            var invoice = Invoice.Create(
                tenantId: tenantId,
                studentId: studentId,
                academicYearId: request.AcademicYearId,
                invoiceNumber: invoiceNumber,
                description: request.Description,
                amount: request.Amount,
                dueDate: request.DueDate,
                month: request.Month,
                year: request.Year,
                discount: request.DefaultDiscount);

            newInvoices.Add(invoice);
        }

        _db.Invoices.AddRange(newInvoices);
        await _db.SaveChangesAsync(cancellationToken);

        var totalBilled = newInvoices.Sum(i => i.Amount - i.Discount);

        return new BulkInvoiceResult(
            Created: newInvoices.Count,
            Skipped: alreadyInvoiced.Count,
            TotalBilled: totalBilled,
            Period: $"{request.Month:D2}/{request.Year}");
    }
}
