using FluentValidation;

namespace SchoolERP.Application.Teachers.Commands.AssignTeacher;

public class AssignTeacherCommandValidator : AbstractValidator<AssignTeacherCommand>
{
    public AssignTeacherCommandValidator()
    {
        RuleFor(x => x.TeacherId).NotEmpty();
        RuleFor(x => x.SectionId).NotEmpty();
        RuleFor(x => x.SubjectId).NotEmpty();
        RuleFor(x => x.AcademicYearId).NotEmpty();
    }
}
