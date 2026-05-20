using FluentValidation;

namespace SchoolERP.Application.Guardians.Commands.CreateGuardian;

public class CreateGuardianCommandValidator : AbstractValidator<CreateGuardianCommand>
{
    public CreateGuardianCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(60);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(60);

        RuleFor(x => x.NationalId)
            .Length(11).WithMessage("La cédula debe tener 11 dígitos.")
            .Matches(@"^\d{11}$").WithMessage("La cédula solo debe contener dígitos.")
            .When(x => x.NationalId != null);

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("El correo electrónico no es válido.")
            .MaximumLength(255)
            .When(x => x.Email != null);

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .When(x => x.Phone != null);

        RuleFor(x => x)
            .Must(x => x.Email != null || x.Phone != null)
            .WithMessage("Se requiere al menos un medio de contacto (email o teléfono).");
    }
}
