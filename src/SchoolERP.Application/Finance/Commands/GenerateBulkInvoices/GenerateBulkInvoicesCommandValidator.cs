using FluentValidation;

namespace SchoolERP.Application.Finance.Commands.GenerateBulkInvoices;

public class GenerateBulkInvoicesCommandValidator : AbstractValidator<GenerateBulkInvoicesCommand>
{
    public GenerateBulkInvoicesCommandValidator()
    {
        RuleFor(x => x.SchoolId).NotEmpty();
        RuleFor(x => x.AcademicYearId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.DueDate).GreaterThan(DateTime.UtcNow.Date);
        RuleFor(x => x.Month).InclusiveBetween(1, 12);
        RuleFor(x => x.Year).InclusiveBetween(2020, 2100);
        RuleFor(x => x.DefaultDiscount).GreaterThanOrEqualTo(0);
    }
}
