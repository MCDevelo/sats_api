using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Models;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Reports.Queries.GetFinancialReport;

public class GetFinancialReportQueryHandler
    : IRequestHandler<GetFinancialReportQuery, ErrorOr<ReportResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IReportGeneratorService _reportGenerator;

    public GetFinancialReportQueryHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        IReportGeneratorService reportGenerator)
    {
        _db = db;
        _currentUser = currentUser;
        _reportGenerator = reportGenerator;
    }

    public async Task<ErrorOr<ReportResult>> Handle(
        GetFinancialReportQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var school = await _db.Schools
            .FirstOrDefaultAsync(s =>
                s.Id == request.SchoolId &&
                s.TenantId == tenantId, cancellationToken);

        if (school is null)
            return Error.NotFound(description: "Centro educativo no encontrado.");

        // Invoices don't have SchoolId — filter through Student.SchoolId.
        // DateFrom/DateTo map to Year+Month range.
        var invoices = await _db.Invoices
            .Include(i => i.Student)
            .Where(i =>
                i.TenantId == tenantId &&
                i.Student.SchoolId == request.SchoolId &&
                i.Status != InvoiceStatus.Cancelled &&
                (i.Year > request.DateFrom.Year ||
                 (i.Year == request.DateFrom.Year && i.Month >= request.DateFrom.Month)) &&
                (i.Year < request.DateTo.Year ||
                 (i.Year == request.DateTo.Year && i.Month <= request.DateTo.Month)))
            .ToListAsync(cancellationToken);

        var invoiceIds = invoices.Select(i => i.Id).ToHashSet();

        var payments = await _db.Payments
            .Where(p =>
                p.TenantId == tenantId &&
                invoiceIds.Contains(p.InvoiceId))
            .ToListAsync(cancellationToken);

        // Totals — TotalAmount = Amount - Discount
        var totalBilled    = invoices.Sum(i => i.Amount - i.Discount);
        var totalCollected = payments.Sum(p => p.Amount);
        var totalPending   = invoices
            .Where(i => i.Status is InvoiceStatus.Pending or InvoiceStatus.Partial)
            .Sum(i => i.Balance);
        var totalOverdue   = invoices
            .Where(i => i.IsOverdue || i.Status == InvoiceStatus.Overdue)
            .Sum(i => i.Balance);
        var collectionRate = totalBilled > 0
            ? Math.Round(totalCollected / totalBilled * 100, 1)
            : 0m;

        // Monthly breakdown — group by Year+Month
        var monthly = invoices
            .GroupBy(i => new { i.Year, i.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g =>
            {
                var billed    = g.Sum(i => i.Amount - i.Discount);
                var invoicesInGroup = g.Select(i => i.Id).ToHashSet();
                var collected = payments
                    .Where(p => invoicesInGroup.Contains(p.InvoiceId))
                    .Sum(p => p.Amount);
                return new MonthlyFinancialRow(
                    Month: new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                    Billed: billed,
                    Collected: collected,
                    Pending: Math.Max(billed - collected, 0));
            }).ToList();

        // Payment method breakdown
        var paymentMethods = payments
            .GroupBy(p => p.Method.ToString())
            .Select(g => new { Method = g.Key, Amount = g.Sum(p => p.Amount) })
            .OrderByDescending(x => x.Amount)
            .Select(x => new PaymentMethodRow(
                x.Method,
                x.Amount,
                totalCollected > 0 ? Math.Round(x.Amount / totalCollected * 100, 1) : 0m))
            .ToList();

        var data = new FinancialReportData(
            SchoolName: school.Name,
            DateFrom: new DateTime(request.DateFrom.Year, request.DateFrom.Month, 1),
            DateTo: new DateTime(request.DateTo.Year, request.DateTo.Month,
                DateTime.DaysInMonth(request.DateTo.Year, request.DateTo.Month)),
            GeneratedAt: DateTime.UtcNow,
            TotalBilled: totalBilled,
            TotalCollected: totalCollected,
            TotalPending: totalPending,
            TotalOverdue: totalOverdue,
            CollectionRate: collectionRate,
            Monthly: monthly,
            PaymentMethods: paymentMethods);

        var pdf = _reportGenerator.GenerateFinancialReport(data);
        var fileName = $"financiero_{request.DateFrom:yyyyMM}_{request.DateTo:yyyyMM}_{DateTime.UtcNow:yyyyMMdd}.pdf";

        return new ReportResult(pdf, fileName);
    }
}
