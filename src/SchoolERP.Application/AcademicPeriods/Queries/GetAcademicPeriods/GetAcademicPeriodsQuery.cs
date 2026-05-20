using ErrorOr;
using MediatR;

namespace SchoolERP.Application.AcademicPeriods.Queries.GetAcademicPeriods;

public record GetAcademicPeriodsQuery(Guid AcademicYearId) : IRequest<ErrorOr<List<AcademicPeriodResult>>>;

public record AcademicPeriodResult(
    Guid Id,
    Guid AcademicYearId,
    string Name,
    int PeriodNumber,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive,
    bool GradesPublished,
    DateTime CreatedAt);
