using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Grades.Commands.UpdateGrade;

public record UpdateGradeCommand(
    Guid GradeEntryId,
    decimal Score,
    string? Comments) : IRequest<ErrorOr<Success>>;
