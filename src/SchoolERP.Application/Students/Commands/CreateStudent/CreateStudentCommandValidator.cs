using FluentValidation;

namespace SchoolERP.Application.Students.Commands.CreateStudent;

public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator()
    {
        RuleFor(x => x.SchoolId).NotEmpty();

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(60);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es requerido.")
            .MaximumLength(60);

        RuleFor(x => x.SecondLastName).MaximumLength(60).When(x => x.SecondLastName is not null);

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .LessThan(DateTime.UtcNow).WithMessage("La fecha de nacimiento debe ser en el pasado.")
            .GreaterThan(DateTime.UtcNow.AddYears(-25)).WithMessage("Edad máxima permitida: 25 años.");

        RuleFor(x => x.NationalId)
            .MaximumLength(11)
            .Matches(@"^\d{11}$").WithMessage("La cédula debe tener 11 dígitos.")
            .When(x => x.NationalId is not null);

        RuleFor(x => x.Nse).MaximumLength(20).When(x => x.Nse is not null);
        RuleFor(x => x.StudentCode).MaximumLength(20).When(x => x.StudentCode is not null);
        RuleFor(x => x.BloodType)
            .Must(b => new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" }.Contains(b))
            .WithMessage("Tipo de sangre inválido.")
            .When(x => x.BloodType is not null);
        RuleFor(x => x.Province).MaximumLength(100).When(x => x.Province is not null);
        RuleFor(x => x.Municipality).MaximumLength(100).When(x => x.Municipality is not null);
        RuleFor(x => x.Phone).MaximumLength(20).When(x => x.Phone is not null);
        RuleFor(x => x.Allergies).MaximumLength(1000).When(x => x.Allergies is not null);
        RuleFor(x => x.MedicalNotes).MaximumLength(2000).When(x => x.MedicalNotes is not null);
        RuleFor(x => x.HealthInsurance).MaximumLength(100).When(x => x.HealthInsurance is not null);
        RuleFor(x => x.HealthInsuranceNumber).MaximumLength(30).When(x => x.HealthInsuranceNumber is not null);
    }
}
