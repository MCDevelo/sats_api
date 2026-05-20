using FluentValidation;

namespace SchoolERP.Application.Teachers.Commands.CreateTeacher;

public class CreateTeacherCommandValidator : AbstractValidator<CreateTeacherCommand>
{
    public CreateTeacherCommandValidator()
    {
        RuleFor(x => x.SchoolId).NotEmpty();

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(60);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es requerido.")
            .MaximumLength(60);

        RuleFor(x => x.HireDate)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(90))
            .WithMessage("La fecha de contratación no puede ser más de 90 días en el futuro.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email inválido.")
            .MaximumLength(255)
            .When(x => x.Email is not null);

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .When(x => x.Phone is not null);

        RuleFor(x => x.NationalId)
            .MaximumLength(11)
            .Matches(@"^\d{11}$").WithMessage("La cédula debe tener 11 dígitos.")
            .When(x => x.NationalId is not null);

        RuleFor(x => x.MinerdCode).MaximumLength(20).When(x => x.MinerdCode is not null);
        RuleFor(x => x.TeacherCode).MaximumLength(30).When(x => x.TeacherCode is not null);
        RuleFor(x => x.AcademicTitle).MaximumLength(100).When(x => x.AcademicTitle is not null);
        RuleFor(x => x.Specialization).MaximumLength(500).When(x => x.Specialization is not null);
        RuleFor(x => x.Qualifications).MaximumLength(1000).When(x => x.Qualifications is not null);
        RuleFor(x => x.Address).MaximumLength(500).When(x => x.Address is not null);

        RuleFor(x => x.WorkingHoursPerWeek)
            .InclusiveBetween(1, 60).WithMessage("Las horas de trabajo por semana deben estar entre 1 y 60.");

        RuleFor(x => x.ContractEndDate)
            .GreaterThan(x => x.HireDate).WithMessage("La fecha de fin del contrato debe ser posterior a la fecha de contratación.")
            .When(x => x.ContractEndDate.HasValue);
    }
}
