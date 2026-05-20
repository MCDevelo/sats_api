using ErrorOr;
using MediatR;
using SchoolERP.Application.Common.Models;

namespace SchoolERP.Application.Communications.Queries.GetInbox;

public record GetInboxQuery(
    bool? OnlyUnread = null,
    int Page = 1,
    int PageSize = 20) : IRequest<ErrorOr<PagedResult<InboxMessageResult>>>;

public record InboxMessageResult(
    Guid Id,
    Guid SenderId,
    string SenderName,
    string Subject,
    string Body,
    bool IsRead,
    DateTime? ReadAt,
    Guid? ParentMessageId,
    DateTime CreatedAt);
