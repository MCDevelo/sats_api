using FluentValidation;

namespace SchoolERP.Application.Grades.Commands.RecordGrade;

public class RecordGradeCommandValidator : AbstractValidator<RecordGradeCommand>
{
    public RecordGradeCommandValidator()
    {
        RuleFor(x => x.EvaluationPlanId).NotEmpty();

        RuleFor(x => x.Entries)
            .NotEmpty().WithMessage("Debe incluir al menos una calificación.")
            .Must(e => e.Select(x => x.StudentId).Distinct().Count() == e.Count)
            .WithMessage("No puede haber estudiantes duplicados.");

        RuleForEach(x => x.Entries).ChildRules(entry =>
        {
            entry.RuleFor(e => e.StudentId).NotEmpty();
            entry.RuleFor(e => e.Score)
                .InclusiveBetween(0, 100).WithMessage("La calificación debe estar entre 0 y 100.");
            entry.RuleFor(e => e.Comments).MaximumLength(500).When(e => e.Comments is not null);
        });
    }
}
