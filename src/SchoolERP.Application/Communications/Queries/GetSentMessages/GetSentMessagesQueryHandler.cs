using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Models;

namespace SchoolERP.Application.Communications.Queries.GetSentMessages;

public class GetSentMessagesQueryHandler
    : IRequestHandler<GetSentMessagesQuery, ErrorOr<PagedResult<SentMessageResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetSentMessagesQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<PagedResult<SentMessageResult>>> Handle(
        GetSentMessagesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;
        var tenantId = _currentUser.TenantId!.Value;

        var query = _db.Messages
            .AsNoTracking()
            .Include(m => m.Recipient)
            .Where(m => m.SenderId == userId && m.TenantId == tenantId);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new SentMessageResult(
                m.Id,
                m.RecipientId,
                m.Recipient.Email ?? string.Empty,
                m.Subject,
                m.Body,
                m.IsRead,
                m.ReadAt,
                m.ParentMessageId,
                m.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<SentMessageResult>(items, total, request.Page, request.PageSize);
    }
}
