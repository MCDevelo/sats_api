using FluentValidation;

namespace SchoolERP.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Role).IsInEnum();
        RuleFor(x => x.Email).EmailAddress().MaximumLength(256).When(x => x.Email is not null);
        RuleFor(x => x.Phone).MaximumLength(20).When(x => x.Phone is not null);

        RuleFor(x => x)
            .Must(x => x.Email is not null || x.Phone is not null)
            .WithName("Contact")
            .WithMessage("Se requiere email o teléfono.");
    }
}
