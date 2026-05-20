using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Subjects.Queries.GetSubjects;

public class GetSubjectsQueryHandler : IRequestHandler<GetSubjectsQuery, ErrorOr<List<SubjectResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetSubjectsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<List<SubjectResult>>> Handle(GetSubjectsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var gradeLevelExists = await _db.GradeLevels
            .AnyAsync(g => g.Id == request.GradeLevelId && g.TenantId == tenantId, cancellationToken);

        if (!gradeLevelExists)
            return Error.NotFound("GradeLevel.NotFound", "Nivel de grado no encontrado.");

        var query = _db.Subjects.Where(s => s.GradeLevelId == request.GradeLevelId && s.TenantId == tenantId);

        if (request.IsActive.HasValue)
            query = query.Where(s => s.IsActive == request.IsActive.Value);

        var items = await query
            .OrderBy(s => s.Name)
            .Select(s => new SubjectResult(
                s.Id,
                s.GradeLevelId,
                s.SchoolId,
                s.Name,
                s.Code,
                s.Description,
                s.WeeklyHours,
                s.IsRequired,
                s.IsActive,
                s.CreatedAt))
            .ToListAsync(cancellationToken);

        return items;
    }
}
