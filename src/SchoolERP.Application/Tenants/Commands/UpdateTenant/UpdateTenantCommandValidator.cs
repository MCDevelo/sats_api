using FluentValidation;

namespace SchoolERP.Application.Tenants.Commands.UpdateTenant;

public class UpdateTenantCommandValidator : AbstractValidator<UpdateTenantCommand>
{
    public UpdateTenantCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.LegalName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ContactEmail).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Rnc).MaximumLength(20).When(x => x.Rnc is not null);
        RuleFor(x => x.ContactPhone).MaximumLength(20).When(x => x.ContactPhone is not null);
    }
}
