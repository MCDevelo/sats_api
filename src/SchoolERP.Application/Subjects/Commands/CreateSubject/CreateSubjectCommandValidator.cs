using FluentValidation;

namespace SchoolERP.Application.Subjects.Commands.CreateSubject;

public class CreateSubjectCommandValidator : AbstractValidator<CreateSubjectCommand>
{
    public CreateSubjectCommandValidator()
    {
        RuleFor(x => x.SchoolId).NotEmpty();
        RuleFor(x => x.GradeLevelId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.WeeklyHours).InclusiveBetween(1, 40);
        RuleFor(x => x.Code).MaximumLength(20).When(x => x.Code is not null);
    }
}
