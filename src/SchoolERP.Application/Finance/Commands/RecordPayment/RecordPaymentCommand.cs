using ErrorOr;
using MediatR;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Finance.Commands.RecordPayment;

public record RecordPaymentCommand(
    Guid InvoiceId,
    decimal Amount,
    PaymentMethod Method,
    string? Reference = null,
    DateTime? PaidAt = null,
    string? Notes = null) : IRequest<ErrorOr<PaymentResult>>;

public record PaymentResult(
    Guid PaymentId,
    Guid InvoiceId,
    string InvoiceNumber,
    string StudentFullName,
    decimal AmountPaid,
    string Method,
    string? Reference,
    DateTime PaidAt,
    decimal RemainingBalance,
    string InvoiceStatus);
