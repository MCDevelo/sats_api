using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Grades.Commands.PublishGrades;

public record PublishGradesCommand(Guid AcademicPeriodId) : IRequest<ErrorOr<PublishGradesResult>>;

public record PublishGradesResult(
    Guid AcademicPeriodId,
    string PeriodName,
    int TotalGradeEntries,
    DateTime PublishedAt);
