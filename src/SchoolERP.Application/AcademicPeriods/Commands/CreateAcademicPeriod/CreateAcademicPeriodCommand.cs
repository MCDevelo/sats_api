using ErrorOr;
using MediatR;

namespace SchoolERP.Application.AcademicPeriods.Commands.CreateAcademicPeriod;

public record CreateAcademicPeriodCommand(
    Guid AcademicYearId,
    string Name,
    int PeriodNumber,
    DateTime StartDate,
    DateTime EndDate) : IRequest<ErrorOr<Guid>>;
