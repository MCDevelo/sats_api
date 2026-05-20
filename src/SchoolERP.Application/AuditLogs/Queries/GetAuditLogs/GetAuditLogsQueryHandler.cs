using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Models;

namespace SchoolERP.Application.AuditLogs.Queries.GetAuditLogs;

public class GetAuditLogsQueryHandler
    : IRequestHandler<GetAuditLogsQuery, ErrorOr<PagedResult<AuditLogResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetAuditLogsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<PagedResult<AuditLogResult>>> Handle(
        GetAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var query = _db.AuditLogs
            .Where(a => a.TenantId == tenantId || a.TenantId == null);

        if (!string.IsNullOrWhiteSpace(request.EntityName))
            query = query.Where(a =>
                a.EntityName.ToLower().Contains(request.EntityName.ToLower()));

        if (!string.IsNullOrWhiteSpace(request.Action))
            query = query.Where(a => a.Action == request.Action.ToUpper());

        if (!string.IsNullOrWhiteSpace(request.EntityId))
            query = query.Where(a => a.EntityId == request.EntityId);

        if (request.UserId.HasValue)
            query = query.Where(a => a.UserId == request.UserId);

        if (request.DateFrom.HasValue)
            query = query.Where(a => a.CreatedAt >= request.DateFrom.Value);

        if (request.DateTo.HasValue)
            query = query.Where(a => a.CreatedAt <= request.DateTo.Value);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new AuditLogResult(
                a.Id,
                a.UserId,
                a.Action,
                a.EntityName,
                a.EntityId,
                a.OldValues,
                a.NewValues,
                a.IpAddress,
                a.UserAgent,
                a.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<AuditLogResult>(items, total, request.Page, request.PageSize);
    }
}
