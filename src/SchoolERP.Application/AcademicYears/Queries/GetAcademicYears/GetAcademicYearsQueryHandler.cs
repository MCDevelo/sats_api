using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.AcademicYears.Queries.GetAcademicYears;

public class GetAcademicYearsQueryHandler : IRequestHandler<GetAcademicYearsQuery, ErrorOr<List<AcademicYearResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetAcademicYearsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<List<AcademicYearResult>>> Handle(GetAcademicYearsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var schoolExists = await _db.Schools
            .AnyAsync(s => s.Id == request.SchoolId && s.TenantId == tenantId, cancellationToken);

        if (!schoolExists)
            return Error.NotFound("School.NotFound", "Escuela no encontrada.");

        var items = await _db.AcademicYears
            .Where(y => y.SchoolId == request.SchoolId && y.TenantId == tenantId)
            .OrderByDescending(y => y.StartDate)
            .Select(y => new AcademicYearResult(
                y.Id,
                y.SchoolId,
                y.Name,
                y.StartDate,
                y.EndDate,
                y.IsActive,
                y.IsCurrent,
                y.Periods.Count,
                y.CreatedAt))
            .ToListAsync(cancellationToken);

        return items;
    }
}
