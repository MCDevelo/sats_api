using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Sections.Commands.AssignHomeTeacher;

public record AssignHomeTeacherCommand(
    Guid SectionId,
    Guid? TeacherId) : IRequest<ErrorOr<Success>>;
