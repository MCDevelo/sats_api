using ErrorOr;
using MediatR;

namespace SchoolERP.Application.AcademicYears.Queries.GetAcademicYears;

public record GetAcademicYearsQuery(Guid SchoolId) : IRequest<ErrorOr<List<AcademicYearResult>>>;

public record AcademicYearResult(
    Guid Id,
    Guid SchoolId,
    string Name,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive,
    bool IsCurrent,
    int PeriodCount,
    DateTime CreatedAt);
