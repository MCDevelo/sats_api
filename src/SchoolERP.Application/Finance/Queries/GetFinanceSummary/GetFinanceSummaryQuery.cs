using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Finance.Queries.GetFinanceSummary;

public record GetFinanceSummaryQuery(
    Guid SchoolId,
    Guid AcademicYearId,
    int? Month = null) : IRequest<ErrorOr<FinanceSummaryResult>>;

public record FinanceSummaryResult(
    Guid SchoolId,
    string SchoolName,
    string AcademicYear,
    string Period,
    decimal TotalBilled,
    decimal TotalDiscounts,
    decimal TotalCollected,
    decimal TotalPending,
    decimal TotalOverdue,
    int InvoiceCount,
    int PaidCount,
    int PartialCount,
    int PendingCount,
    int OverdueCount,
    int CancelledCount,
    decimal CollectionRate,
    IReadOnlyList<MonthlyBreakdown> MonthlyBreakdowns,
    IReadOnlyList<PaymentMethodBreakdown> PaymentMethodBreakdowns);

public record MonthlyBreakdown(
    int Month,
    string MonthName,
    decimal Billed,
    decimal Collected,
    decimal Pending,
    int InvoiceCount);

public record PaymentMethodBreakdown(
    string Method,
    decimal Amount,
    int Count,
    decimal Percentage);
