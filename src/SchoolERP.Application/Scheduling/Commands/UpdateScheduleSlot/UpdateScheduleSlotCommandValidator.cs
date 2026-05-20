using FluentValidation;

namespace SchoolERP.Application.Scheduling.Commands.UpdateScheduleSlot;

public class UpdateScheduleSlotCommandValidator : AbstractValidator<UpdateScheduleSlotCommand>
{
    private static readonly DayOfWeek[] SchoolDays =
        [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday];

    public UpdateScheduleSlotCommandValidator()
    {
        RuleFor(x => x.SlotId).NotEmpty();

        RuleFor(x => x.Day)
            .Must(d => SchoolDays.Contains(d))
            .WithMessage("El día debe ser de lunes a viernes.");

        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime)
            .WithMessage("La hora de inicio debe ser anterior a la hora de fin.");

        RuleFor(x => x.Room)
            .MaximumLength(50)
            .When(x => x.Room is not null);
    }
}
