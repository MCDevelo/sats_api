using FluentValidation;

namespace SchoolERP.Application.Guardians.Commands.UpdateGuardian;

public class UpdateGuardianCommandValidator : AbstractValidator<UpdateGuardianCommand>
{
    public UpdateGuardianCommandValidator()
    {
        RuleFor(x => x.GuardianId).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(60);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(60);

        RuleFor(x => x.Email)
            .EmailAddress().MaximumLength(255)
            .When(x => x.Email != null);

        RuleFor(x => x)
            .Must(x => x.Email != null || x.Phone != null)
            .WithMessage("Se requiere al menos un medio de contacto (email o teléfono).");
    }
}
