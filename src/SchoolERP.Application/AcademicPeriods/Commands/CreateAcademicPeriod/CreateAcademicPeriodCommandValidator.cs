using FluentValidation;

namespace SchoolERP.Application.AcademicPeriods.Commands.CreateAcademicPeriod;

public class CreateAcademicPeriodCommandValidator : AbstractValidator<CreateAcademicPeriodCommand>
{
    public CreateAcademicPeriodCommandValidator()
    {
        RuleFor(x => x.AcademicYearId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PeriodNumber).InclusiveBetween(1, 12);
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty().GreaterThan(x => x.StartDate)
            .WithMessage("La fecha de fin debe ser posterior a la fecha de inicio.");
    }
}
