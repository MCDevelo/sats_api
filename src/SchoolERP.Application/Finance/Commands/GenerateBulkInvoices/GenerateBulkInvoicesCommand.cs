using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Finance.Commands.GenerateBulkInvoices;

public record GenerateBulkInvoicesCommand(
    Guid SchoolId,
    Guid AcademicYearId,
    int Month,
    int Year,
    string Description,
    decimal Amount,
    DateTime DueDate,
    decimal DefaultDiscount = 0) : IRequest<ErrorOr<BulkInvoiceResult>>;

public record BulkInvoiceResult(
    int Created,
    int Skipped,
    decimal TotalBilled,
    string Period);
