using FluentValidation;

namespace SchoolERP.Application.Attendance.Commands.ExcuseAbsence;

public class ExcuseAbsenceCommandValidator : AbstractValidator<ExcuseAbsenceCommand>
{
    public ExcuseAbsenceCommandValidator()
    {
        RuleFor(x => x.RecordId).NotEmpty();
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Debe indicar el motivo de la excusa.")
            .MaximumLength(500);
    }
}
