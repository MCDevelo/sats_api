using FluentValidation;

namespace SchoolERP.Application.Teachers.Commands.UpdateTeacher;

public class UpdateTeacherCommandValidator : AbstractValidator<UpdateTeacherCommand>
{
    public UpdateTeacherCommandValidator()
    {
        RuleFor(x => x.TeacherId).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(60);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(60);

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email inválido.")
            .MaximumLength(255)
            .When(x => x.Email is not null);

        RuleFor(x => x.Phone).MaximumLength(20).When(x => x.Phone is not null);
        RuleFor(x => x.Address).MaximumLength(500).When(x => x.Address is not null);
        RuleFor(x => x.Specialization).MaximumLength(500).When(x => x.Specialization is not null);
        RuleFor(x => x.Qualifications).MaximumLength(1000).When(x => x.Qualifications is not null);
        RuleFor(x => x.AcademicTitle).MaximumLength(100).When(x => x.AcademicTitle is not null);

        RuleFor(x => x.WorkingHoursPerWeek)
            .InclusiveBetween(1, 60).WithMessage("Las horas de trabajo por semana deben estar entre 1 y 60.");

        RuleFor(x => x.ContractEndDate)
            .GreaterThan(DateTime.UtcNow.Date).WithMessage("La fecha de fin del contrato debe ser futura.")
            .When(x => x.ContractEndDate.HasValue);
    }
}
