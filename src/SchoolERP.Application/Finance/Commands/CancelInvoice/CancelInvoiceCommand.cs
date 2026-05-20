using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Finance.Commands.CancelInvoice;

public record CancelInvoiceCommand(
    Guid InvoiceId,
    string Reason) : IRequest<ErrorOr<Success>>;
