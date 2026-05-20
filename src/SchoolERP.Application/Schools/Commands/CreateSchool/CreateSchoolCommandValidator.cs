using FluentValidation;

namespace SchoolERP.Application.Schools.Commands.CreateSchool;

public class CreateSchoolCommandValidator : AbstractValidator<CreateSchoolCommand>
{
    public CreateSchoolCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.LevelType).IsInEnum();
        RuleFor(x => x.Email).EmailAddress().MaximumLength(256).When(x => x.Email is not null);
        RuleFor(x => x.CodeMinerd).MaximumLength(20).When(x => x.CodeMinerd is not null);
    }
}
