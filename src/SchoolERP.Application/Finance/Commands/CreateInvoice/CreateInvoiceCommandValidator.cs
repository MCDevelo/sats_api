using FluentValidation;

namespace SchoolERP.Application.Finance.Commands.CreateInvoice;

public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.AcademicYearId).NotEmpty();

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es requerida.")
            .MaximumLength(500);

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a 0.");

        RuleFor(x => x.DueDate)
            .NotEmpty()
            .GreaterThan(DateTime.UtcNow.Date).WithMessage("La fecha de vencimiento debe ser futura.");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12).WithMessage("El mes debe estar entre 1 y 12.");

        RuleFor(x => x.Year)
            .InclusiveBetween(2020, 2100).WithMessage("Año inválido.");

        RuleFor(x => x.Discount)
            .GreaterThanOrEqualTo(0)
            .Must((cmd, discount) => discount < cmd.Amount)
            .WithMessage("El descuento no puede ser mayor o igual al monto total.")
            .When(x => x.Discount > 0);

        RuleFor(x => x.Ncf)
            .MaximumLength(20)
            .When(x => x.Ncf is not null);
    }
}
