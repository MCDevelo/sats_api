using FluentValidation;

namespace SchoolERP.Application.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Role).IsInEnum();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(128);
        RuleFor(x => x.Email).EmailAddress().MaximumLength(256).When(x => x.Email is not null);
        RuleFor(x => x.Phone).MaximumLength(20).When(x => x.Phone is not null);

        RuleFor(x => x)
            .Must(x => x.Email is not null || x.Phone is not null)
            .WithName("Contact")
            .WithMessage("Se requiere email o teléfono.");
    }
}
