using FluentValidation;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Communications.Commands.CreateAnnouncement;

public class CreateAnnouncementCommandValidator : AbstractValidator<CreateAnnouncementCommand>
{
    public CreateAnnouncementCommandValidator()
    {
        RuleFor(x => x.SchoolId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Body).NotEmpty().MaximumLength(5000);

        RuleFor(x => x.AudienceId)
            .NotEmpty()
            .WithMessage("AudienceId es requerido cuando la audiencia es Section o GradeLevel.")
            .When(x => x.Audience == AnnouncementAudience.Section || x.Audience == AnnouncementAudience.GradeLevel);

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("La fecha de expiración debe ser futura.")
            .When(x => x.ExpiresAt.HasValue);
    }
}
