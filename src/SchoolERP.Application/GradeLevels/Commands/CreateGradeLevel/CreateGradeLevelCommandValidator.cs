using FluentValidation;

namespace SchoolERP.Application.GradeLevels.Commands.CreateGradeLevel;

public class CreateGradeLevelCommandValidator : AbstractValidator<CreateGradeLevelCommand>
{
    public CreateGradeLevelCommandValidator()
    {
        RuleFor(x => x.SchoolId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Order).GreaterThan(0);
        RuleFor(x => x.EducationLevel).IsInEnum();
    }
}
