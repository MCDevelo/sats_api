using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.GradeLevels.Queries.GetGradeLevels;

public class GetGradeLevelsQueryHandler : IRequestHandler<GetGradeLevelsQuery, ErrorOr<List<GradeLevelResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetGradeLevelsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<List<GradeLevelResult>>> Handle(GetGradeLevelsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var schoolExists = await _db.Schools
            .AnyAsync(s => s.Id == request.SchoolId && s.TenantId == tenantId, cancellationToken);

        if (!schoolExists)
            return Error.NotFound("School.NotFound", "Escuela no encontrada.");

        var items = await _db.GradeLevels
            .Where(g => g.SchoolId == request.SchoolId && g.TenantId == tenantId)
            .OrderBy(g => g.Order)
            .Select(g => new GradeLevelResult(
                g.Id,
                g.SchoolId,
                g.Name,
                g.Order,
                g.EducationLevel,
                g.IsActive,
                g.Subjects.Count(s => s.IsActive),
                g.Sections.Count(s => s.IsActive),
                g.CreatedAt))
            .ToListAsync(cancellationToken);

        return items;
    }
}
