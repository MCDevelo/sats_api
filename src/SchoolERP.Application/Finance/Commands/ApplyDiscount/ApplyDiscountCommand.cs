using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Finance.Commands.ApplyDiscount;

public record ApplyDiscountCommand(
    Guid InvoiceId,
    decimal Discount,
    string Reason) : IRequest<ErrorOr<Success>>;
