using FluentValidation;

namespace SchoolERP.Application.Finance.Commands.RecordPayment;

public class RecordPaymentCommandValidator : AbstractValidator<RecordPaymentCommand>
{
    public RecordPaymentCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("El monto del pago debe ser mayor a 0.");

        RuleFor(x => x.Reference)
            .MaximumLength(100)
            .When(x => x.Reference is not null);

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => x.Notes is not null);

        RuleFor(x => x.PaidAt)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("La fecha de pago no puede ser futura.")
            .When(x => x.PaidAt.HasValue);
    }
}
