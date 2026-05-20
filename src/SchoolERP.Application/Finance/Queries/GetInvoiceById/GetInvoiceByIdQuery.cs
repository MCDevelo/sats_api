using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Finance.Queries.GetInvoiceById;

public record GetInvoiceByIdQuery(Guid InvoiceId) : IRequest<ErrorOr<InvoiceDetailResult>>;

public record InvoiceDetailResult(
    Guid Id,
    string InvoiceNumber,
    string? Ncf,
    Guid StudentId,
    string StudentFullName,
    string? StudentCode,
    string AcademicYear,
    string Description,
    decimal Amount,
    decimal Discount,
    decimal AmountPaid,
    decimal Balance,
    string Status,
    DateTime DueDate,
    bool IsOverdue,
    int Month,
    int Year,
    DateTime CreatedAt,
    IReadOnlyList<PaymentSummary> Payments);

public record PaymentSummary(
    Guid PaymentId,
    decimal Amount,
    string Method,
    string? Reference,
    DateTime PaidAt,
    string? Notes);
