using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Finance.Commands.CreateInvoice;

public record CreateInvoiceCommand(
    Guid StudentId,
    Guid AcademicYearId,
    string Description,
    decimal Amount,
    DateTime DueDate,
    int Month,
    int Year,
    decimal Discount = 0,
    string? Ncf = null,
    string? Notes = null) : IRequest<ErrorOr<InvoiceResult>>;

public record InvoiceResult(
    Guid Id,
    Guid StudentId,
    string StudentFullName,
    string InvoiceNumber,
    string? Ncf,
    string Description,
    decimal Amount,
    decimal Discount,
    decimal Balance,
    decimal AmountPaid,
    string Status,
    DateTime DueDate,
    int Month,
    int Year,
    DateTime CreatedAt);
