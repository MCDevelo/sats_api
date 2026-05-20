using FluentValidation;

namespace SchoolERP.Application.Sections.Commands.CreateSection;

public class CreateSectionCommandValidator : AbstractValidator<CreateSectionCommand>
{
    public CreateSectionCommandValidator()
    {
        RuleFor(x => x.SchoolId).NotEmpty();
        RuleFor(x => x.GradeLevelId).NotEmpty();
        RuleFor(x => x.AcademicYearId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Shift).IsInEnum();
        RuleFor(x => x.Capacity).InclusiveBetween(1, 100);
        RuleFor(x => x.Classroom).MaximumLength(50).When(x => x.Classroom is not null);
    }
}
