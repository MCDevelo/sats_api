using FluentValidation;

namespace SchoolERP.Application.Notifications.Commands.SendNotification;

public class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationCommandValidator()
    {
        RuleFor(x => x.EventType).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es requerido.")
            .MaximumLength(200);

        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("El cuerpo del mensaje es requerido.")
            .MaximumLength(2000);

        RuleFor(x => x.Channels)
            .NotEmpty().WithMessage("Debe especificar al menos un canal de notificación.");

        RuleFor(x => x.RecipientEmail)
            .EmailAddress().WithMessage("Email del destinatario inválido.")
            .When(x => x.RecipientEmail is not null);

        RuleFor(x => x)
            .Must(x => x.RecipientUserId.HasValue || x.RecipientEmail is not null)
            .WithMessage("Debe especificar un destinatario (UserId o Email).");
    }
}
