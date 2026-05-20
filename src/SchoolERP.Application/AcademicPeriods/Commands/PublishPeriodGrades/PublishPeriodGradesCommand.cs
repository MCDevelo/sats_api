using ErrorOr;
using MediatR;

namespace SchoolERP.Application.AcademicPeriods.Commands.PublishPeriodGrades;

public record PublishPeriodGradesCommand(Guid AcademicPeriodId) : IRequest<ErrorOr<Success>>;
