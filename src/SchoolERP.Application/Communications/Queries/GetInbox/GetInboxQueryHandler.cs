using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Models;

namespace SchoolERP.Application.Communications.Queries.GetInbox;

public class GetInboxQueryHandler
    : IRequestHandler<GetInboxQuery, ErrorOr<PagedResult<InboxMessageResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetInboxQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<PagedResult<InboxMessageResult>>> Handle(
        GetInboxQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;
        var tenantId = _currentUser.TenantId!.Value;

        var query = _db.Messages
            .AsNoTracking()
            .Include(m => m.Sender)
            .Where(m => m.RecipientId == userId && m.TenantId == tenantId);

        if (request.OnlyUnread == true)
            query = query.Where(m => !m.IsRead);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new InboxMessageResult(
                m.Id,
                m.SenderId,
                m.Sender.Email ?? string.Empty,
                m.Subject,
                m.Body,
                m.IsRead,
                m.ReadAt,
                m.ParentMessageId,
                m.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<InboxMessageResult>(items, total, request.Page, request.PageSize);
    }
}
