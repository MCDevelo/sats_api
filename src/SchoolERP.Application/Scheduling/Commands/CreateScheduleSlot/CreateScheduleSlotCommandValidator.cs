using FluentValidation;

namespace SchoolERP.Application.Scheduling.Commands.CreateScheduleSlot;

public class CreateScheduleSlotCommandValidator : AbstractValidator<CreateScheduleSlotCommand>
{
    private static readonly DayOfWeek[] SchoolDays =
        [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday];

    public CreateScheduleSlotCommandValidator()
    {
        RuleFor(x => x.TeacherAssignmentId).NotEmpty();

        RuleFor(x => x.Day)
            .Must(d => SchoolDays.Contains(d))
            .WithMessage("El día debe ser de lunes a viernes.");

        RuleFor(x => x.StartTime)
            .LessThan(x => x.EndTime)
            .WithMessage("La hora de inicio debe ser anterior a la hora de fin.");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage("La hora de fin debe ser posterior a la hora de inicio.");

        RuleFor(x => x.Room)
            .MaximumLength(50)
            .When(x => x.Room is not null);
    }
}
