using ErrorOr;
using MediatR;
using SchoolERP.Application.Common.Models;

namespace SchoolERP.Application.Communications.Queries.GetSentMessages;

public record GetSentMessagesQuery(
    int Page = 1,
    int PageSize = 20) : IRequest<ErrorOr<PagedResult<SentMessageResult>>>;

public record SentMessageResult(
    Guid Id,
    Guid RecipientId,
    string RecipientName,
    string Subject,
    string Body,
    bool IsRead,
    DateTime? ReadAt,
    Guid? ParentMessageId,
    DateTime CreatedAt);
