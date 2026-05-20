using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Communications.Commands.MarkMessageRead;

public class MarkMessageReadCommandHandler
    : IRequestHandler<MarkMessageReadCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public MarkMessageReadCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(
        MarkMessageReadCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId!.Value;
        var tenantId = _currentUser.TenantId!.Value;

        var message = await _db.Messages
            .FirstOrDefaultAsync(m =>
                m.Id == request.MessageId &&
                m.TenantId == tenantId &&
                m.RecipientId == userId,
                cancellationToken);

        if (message is null)
            return Error.NotFound("Message.NotFound", "Mensaje no encontrado.");

        message.MarkRead();
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
