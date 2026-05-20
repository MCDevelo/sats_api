using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Communications.Commands.SendMessage;

public record SendMessageCommand(
    Guid RecipientId,
    string Subject,
    string Body,
    Guid? ParentMessageId = null) : IRequest<ErrorOr<MessageResult>>;

public record MessageResult(
    Guid Id,
    Guid SenderId,
    string SenderName,
    Guid RecipientId,
    string RecipientName,
    string Subject,
    string Body,
    Guid? ParentMessageId,
    DateTime CreatedAt);
