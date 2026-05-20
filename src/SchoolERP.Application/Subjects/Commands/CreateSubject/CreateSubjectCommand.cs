using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Subjects.Commands.CreateSubject;

public record CreateSubjectCommand(
    Guid SchoolId,
    Guid GradeLevelId,
    string Name,
    int WeeklyHours,
    string? Code = null,
    string? Description = null,
    bool IsRequired = true) : IRequest<ErrorOr<Guid>>;
