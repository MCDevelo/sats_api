using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Guardians.Commands.CreateGuardian;

namespace SchoolERP.Application.Guardians.Queries.GetGuardians;

public class GetGuardiansQueryHandler
    : IRequestHandler<GetGuardiansQuery, ErrorOr<PagedResult<GuardianResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetGuardiansQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<PagedResult<GuardianResult>>> Handle(
        GetGuardiansQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var query = _db.Guardians
            .AsNoTracking()
            .Where(g => g.TenantId == tenantId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(g =>
                g.FirstName.ToLower().Contains(search) ||
                g.LastName.ToLower().Contains(search) ||
                (g.NationalId != null && g.NationalId.Contains(search)) ||
                (g.Email != null && g.Email.Contains(search)) ||
                (g.Phone != null && g.Phone.Contains(search)));
        }

        if (request.HasPortalAccount.HasValue)
        {
            query = request.HasPortalAccount.Value
                ? query.Where(g => g.UserId != null)
                : query.Where(g => g.UserId == null);
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(g => g.LastName)
            .ThenBy(g => g.FirstName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(g => new GuardianResult(
                g.Id,
                g.FirstName,
                g.LastName,
                g.FirstName + " " + g.LastName,
                g.NationalId,
                g.Email,
                g.Phone,
                g.PhoneSecondary,
                g.WhatsApp,
                g.Address,
                g.Occupation,
                g.Workplace,
                g.IsFinancialResponsible,
                g.Gender != null ? g.Gender.ToString() : null,
                g.UserId,
                g.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<GuardianResult>(items, total, request.Page, request.PageSize);
    }
}
