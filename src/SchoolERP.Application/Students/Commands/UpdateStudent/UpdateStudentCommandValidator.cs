using FluentValidation;

namespace SchoolERP.Application.Students.Commands.UpdateStudent;

public class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
{
    public UpdateStudentCommandValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(60);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es requerido.")
            .MaximumLength(60);

        RuleFor(x => x.SecondLastName).MaximumLength(60).When(x => x.SecondLastName is not null);
        RuleFor(x => x.Address).MaximumLength(500).When(x => x.Address is not null);
        RuleFor(x => x.Province).MaximumLength(100).When(x => x.Province is not null);
        RuleFor(x => x.Municipality).MaximumLength(100).When(x => x.Municipality is not null);
        RuleFor(x => x.Phone).MaximumLength(20).When(x => x.Phone is not null);
        RuleFor(x => x.Allergies).MaximumLength(1000).When(x => x.Allergies is not null);
        RuleFor(x => x.MedicalNotes).MaximumLength(2000).When(x => x.MedicalNotes is not null);
        RuleFor(x => x.HealthInsurance).MaximumLength(100).When(x => x.HealthInsurance is not null);
        RuleFor(x => x.HealthInsuranceNumber).MaximumLength(30).When(x => x.HealthInsuranceNumber is not null);
    }
}
