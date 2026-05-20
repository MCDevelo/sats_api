using FluentValidation;

namespace SchoolERP.Application.Grades.Commands.UpdateGrade;

public class UpdateGradeCommandValidator : AbstractValidator<UpdateGradeCommand>
{
    public UpdateGradeCommandValidator()
    {
        RuleFor(x => x.GradeEntryId).NotEmpty();
        RuleFor(x => x.Score)
            .InclusiveBetween(0, 100).WithMessage("La calificación debe estar entre 0 y 100.");
        RuleFor(x => x.Comments).MaximumLength(500).When(x => x.Comments is not null);
    }
}
