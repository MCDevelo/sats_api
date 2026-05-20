using FluentValidation;

namespace SchoolERP.Application.Sections.Commands.UpdateSection;

public class UpdateSectionCommandValidator : AbstractValidator<UpdateSectionCommand>
{
    public UpdateSectionCommandValidator()
    {
        RuleFor(x => x.SectionId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Shift).IsInEnum();
        RuleFor(x => x.Capacity).InclusiveBetween(1, 100);
        RuleFor(x => x.Classroom).MaximumLength(50).When(x => x.Classroom is not null);
    }
}
