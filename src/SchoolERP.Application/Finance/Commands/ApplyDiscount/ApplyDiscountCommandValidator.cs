using FluentValidation;

namespace SchoolERP.Application.Finance.Commands.ApplyDiscount;

public class ApplyDiscountCommandValidator : AbstractValidator<ApplyDiscountCommand>
{
    public ApplyDiscountCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.Discount).GreaterThan(0).WithMessage("El descuento debe ser mayor a 0.");
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}
