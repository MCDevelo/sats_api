using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Enums;
using System.Globalization;

namespace SchoolERP.Application.Finance.Queries.GetFinanceSummary;

public class GetFinanceSummaryQueryHandler : IRequestHandler<GetFinanceSummaryQuery, ErrorOr<FinanceSummaryResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetFinanceSummaryQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<FinanceSummaryResult>> Handle(GetFinanceSummaryQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var school = await _db.Schools
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.SchoolId && s.TenantId == tenantId, cancellationToken);

        if (school is null)
            return Error.NotFound(description: "Escuela no encontrada.");

        var academicYear = await _db.AcademicYears
            .AsNoTracking()
            .FirstOrDefaultAsync(ay => ay.Id == request.AcademicYearId && ay.TenantId == tenantId, cancellationToken);

        if (academicYear is null)
            return Error.NotFound(description: "Año académico no encontrado.");

        // Invoices scope
        var invoiceQuery = _db.Invoices
            .AsNoTracking()
            .Include(i => i.Student)
            .Where(i =>
                i.TenantId == tenantId &&
                i.AcademicYearId == request.AcademicYearId &&
                i.Student.SchoolId == request.SchoolId);

        if (request.Month.HasValue)
            invoiceQuery = invoiceQuery.Where(i => i.Month == request.Month.Value);

        var invoices = await invoiceQuery.ToListAsync(cancellationToken);

        var active = invoices.Where(i => i.Status != InvoiceStatus.Cancelled).ToList();

        var totalBilled = active.Sum(i => i.Amount - i.Discount);
        var totalCollected = active.Sum(i => i.AmountPaid);
        var totalPending = active.Where(i => i.Status == InvoiceStatus.Pending).Sum(i => i.Balance);
        var totalOverdue = active.Where(i => i.IsOverdue).Sum(i => i.Balance);
        var totalDiscounts = active.Sum(i => i.Discount);
        var collectionRate = totalBilled > 0 ? Math.Round(totalCollected / totalBilled * 100, 1) : 0;

        // Monthly breakdown
        var monthlyBreakdowns = active
            .GroupBy(i => i.Month)
            .OrderBy(g => g.Key)
            .Select(g => new MonthlyBreakdown(
                Month: g.Key,
                MonthName: CultureInfo.GetCultureInfo("es-DO").DateTimeFormat.GetMonthName(g.Key),
                Billed: g.Sum(i => i.Amount - i.Discount),
                Collected: g.Sum(i => i.AmountPaid),
                Pending: g.Sum(i => i.Balance),
                InvoiceCount: g.Count()))
            .ToList();

        // Payment method breakdown
        var paymentIds = invoices.Select(i => i.Id).ToList();
        var payments = await _db.Payments
            .AsNoTracking()
            .Where(p => p.TenantId == tenantId && paymentIds.Contains(p.InvoiceId))
            .ToListAsync(cancellationToken);

        var totalPaymentsAmount = payments.Sum(p => p.Amount);
        var paymentBreakdowns = payments
            .GroupBy(p => p.Method)
            .Select(g => new PaymentMethodBreakdown(
                Method: g.Key.ToString(),
                Amount: g.Sum(p => p.Amount),
                Count: g.Count(),
                Percentage: totalPaymentsAmount > 0
                    ? Math.Round(g.Sum(p => p.Amount) / totalPaymentsAmount * 100, 1) : 0))
            .OrderByDescending(p => p.Amount)
            .ToList();

        var period = request.Month.HasValue
            ? CultureInfo.GetCultureInfo("es-DO").DateTimeFormat.GetMonthName(request.Month.Value)
            : "Año completo";

        return new FinanceSummaryResult(
            SchoolId: school.Id,
            SchoolName: school.Name,
            AcademicYear: academicYear.Name,
            Period: period,
            TotalBilled: totalBilled,
            TotalDiscounts: totalDiscounts,
            TotalCollected: totalCollected,
            TotalPending: totalPending,
            TotalOverdue: totalOverdue,
            InvoiceCount: active.Count,
            PaidCount: active.Count(i => i.Status == InvoiceStatus.Paid),
            PartialCount: active.Count(i => i.Status == InvoiceStatus.Partial),
            PendingCount: active.Count(i => i.Status == InvoiceStatus.Pending),
            OverdueCount: active.Count(i => i.IsOverdue),
            CancelledCount: invoices.Count(i => i.Status == InvoiceStatus.Cancelled),
            CollectionRate: collectionRate,
            MonthlyBreakdowns: monthlyBreakdowns,
            PaymentMethodBreakdowns: paymentBreakdowns);
    }
}
