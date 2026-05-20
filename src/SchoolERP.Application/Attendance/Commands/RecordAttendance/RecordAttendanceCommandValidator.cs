using FluentValidation;

namespace SchoolERP.Application.Attendance.Commands.RecordAttendance;

public class RecordAttendanceCommandValidator : AbstractValidator<RecordAttendanceCommand>
{
    public RecordAttendanceCommandValidator()
    {
        RuleFor(x => x.SectionId).NotEmpty();
        RuleFor(x => x.AcademicPeriodId).NotEmpty();

        RuleFor(x => x.Date)
            .NotEmpty()
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("No se puede registrar asistencia para fechas futuras.");

        RuleFor(x => x.Entries)
            .NotEmpty().WithMessage("Debe incluir al menos un registro de asistencia.")
            .Must(e => e.Select(x => x.StudentId).Distinct().Count() == e.Count)
            .WithMessage("No puede haber estudiantes duplicados en el mismo registro.");

        RuleForEach(x => x.Entries).ChildRules(entry =>
        {
            entry.RuleFor(e => e.StudentId).NotEmpty();
            entry.RuleFor(e => e.Notes).MaximumLength(500).When(e => e.Notes is not null);
        });
    }
}
