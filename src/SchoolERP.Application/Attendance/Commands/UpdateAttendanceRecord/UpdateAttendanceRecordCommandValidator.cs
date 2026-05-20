using FluentValidation;

namespace SchoolERP.Application.Attendance.Commands.UpdateAttendanceRecord;

public class UpdateAttendanceRecordCommandValidator : AbstractValidator<UpdateAttendanceRecordCommand>
{
    public UpdateAttendanceRecordCommandValidator()
    {
        RuleFor(x => x.RecordId).NotEmpty();
        RuleFor(x => x.Notes).MaximumLength(500).When(x => x.Notes is not null);
    }
}
