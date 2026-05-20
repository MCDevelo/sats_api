using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Models;

namespace SchoolERP.Application.Schools.Queries.GetSchools;

public class GetSchoolsQueryHandler : IRequestHandler<GetSchoolsQuery, ErrorOr<PagedResult<SchoolResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetSchoolsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<PagedResult<SchoolResult>>> Handle(GetSchoolsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var query = _db.Schools.Where(s => s.TenantId == tenantId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(s =>
                s.Name.ToLower().Contains(search) ||
                (s.CodeMinerd != null && s.CodeMinerd.ToLower().Contains(search)));
        }

        if (request.IsActive.HasValue)
            query = query.Where(s => s.IsActive == request.IsActive.Value);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new SchoolResult(
                s.Id,
                s.TenantId,
                s.Name,
                s.LegalName,
                s.CodeMinerd,
                s.Rnc,
                s.Province,
                s.Municipality,
                s.Address,
                s.PhonePrimary,
                s.Email,
                s.Website,
                s.LogoUrl,
                s.LevelType,
                s.IsActive,
                s.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<SchoolResult>(items, total, request.Page, request.PageSize);
    }
}
