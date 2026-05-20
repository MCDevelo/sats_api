using FluentValidation;

namespace SchoolERP.Application.Grades.Commands.CreateEvaluationPlan;

public class CreateEvaluationPlanCommandValidator : AbstractValidator<CreateEvaluationPlanCommand>
{
    public CreateEvaluationPlanCommandValidator()
    {
        RuleFor(x => x.SubjectId).NotEmpty();
        RuleFor(x => x.AcademicPeriodId).NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre de la evaluación es requerido.")
            .MaximumLength(100);

        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("El peso debe ser mayor a 0.")
            .LessThanOrEqualTo(100).WithMessage("El peso no puede exceder 100%.");

        RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description is not null);
    }
}
