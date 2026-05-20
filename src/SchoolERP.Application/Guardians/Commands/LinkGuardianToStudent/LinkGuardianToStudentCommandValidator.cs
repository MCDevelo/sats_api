using FluentValidation;

namespace SchoolERP.Application.Guardians.Commands.LinkGuardianToStudent;

public class LinkGuardianToStudentCommandValidator : AbstractValidator<LinkGuardianToStudentCommand>
{
    private static readonly string[] ValidRelationships =
        ["padre", "madre", "tutor", "abuelo", "tio", "hermano", "otro"];

    public LinkGuardianToStudentCommandValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.GuardianId).NotEmpty();
        RuleFor(x => x.Relationship)
            .NotEmpty()
            .Must(r => ValidRelationships.Contains(r.ToLowerInvariant()))
            .WithMessage($"Relación inválida. Valores permitidos: {string.Join(", ", ValidRelationships)}.");
    }
}
