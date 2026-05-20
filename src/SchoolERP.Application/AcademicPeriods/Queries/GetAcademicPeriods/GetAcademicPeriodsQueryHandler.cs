using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.AcademicPeriods.Queries.GetAcademicPeriods;

public class GetAcademicPeriodsQueryHandler : IRequestHandler<GetAcademicPeriodsQuery, ErrorOr<List<AcademicPeriodResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetAcademicPeriodsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<List<AcademicPeriodResult>>> Handle(GetAcademicPeriodsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var yearExists = await _db.AcademicYears
            .AnyAsync(y => y.Id == request.AcademicYearId && y.TenantId == tenantId, cancellationToken);

        if (!yearExists)
            return Error.NotFound("AcademicYear.NotFound", "Año académico no encontrado.");

        var items = await _db.AcademicPeriods
            .Where(p => p.AcademicYearId == request.AcademicYearId)
            .OrderBy(p => p.PeriodNumber)
            .Select(p => new AcademicPeriodResult(
                p.Id,
                p.AcademicYearId,
                p.Name,
                p.PeriodNumber,
                p.StartDate,
                p.EndDate,
                p.IsActive,
                p.GradesPublished,
                p.CreatedAt))
            .ToListAsync(cancellationToken);

        return items;
    }
}
