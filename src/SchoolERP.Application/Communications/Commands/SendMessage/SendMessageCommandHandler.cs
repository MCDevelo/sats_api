using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Communications.Commands.SendMessage;

public class SendMessageCommandHandler
    : IRequestHandler<SendMessageCommand, ErrorOr<MessageResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public SendMessageCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<MessageResult>> Handle(
        SendMessageCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;
        var senderId = _currentUser.UserId!.Value;

        if (senderId == request.RecipientId)
            return Error.Validation("Message.SelfSend", "No puede enviarse un mensaje a sí mismo.");

        var recipient = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.RecipientId && u.TenantId == tenantId && u.IsActive, cancellationToken);

        if (recipient is null)
            return Error.NotFound("User.NotFound", "Destinatario no encontrado.");

        // Validate parent message exists and belongs to this thread
        if (request.ParentMessageId.HasValue)
        {
            var parentExists = await _db.Messages
                .AnyAsync(m =>
                    m.Id == request.ParentMessageId.Value &&
                    m.TenantId == tenantId &&
                    (m.SenderId == senderId || m.RecipientId == senderId),
                    cancellationToken);

            if (!parentExists)
                return Error.NotFound("Message.ParentNotFound", "Mensaje original no encontrado.");
        }

        var sender = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == senderId, cancellationToken);

        var message = Message.Create(
            tenantId: tenantId,
            senderId: senderId,
            recipientId: request.RecipientId,
            subject: request.Subject,
            body: request.Body,
            parentMessageId: request.ParentMessageId);

        _db.Messages.Add(message);
        await _db.SaveChangesAsync(cancellationToken);

        return new MessageResult(
            Id: message.Id,
            SenderId: senderId,
            SenderName: sender?.Email ?? string.Empty,
            RecipientId: recipient.Id,
            RecipientName: recipient.Email ?? string.Empty,
            Subject: message.Subject,
            Body: message.Body,
            ParentMessageId: message.ParentMessageId,
            CreatedAt: message.CreatedAt);
    }
}
