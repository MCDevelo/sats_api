using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateUserCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && u.TenantId == tenantId, cancellationToken);

        if (user is null)
            return Error.NotFound("User.NotFound", "Usuario no encontrado.");

        // Check email uniqueness (excluding self)
        if (request.Email is not null)
        {
            var emailTaken = await _db.Users
                .AnyAsync(u => u.TenantId == tenantId
                    && u.Email == request.Email.ToLowerInvariant()
                    && u.Id != request.UserId, cancellationToken);

            if (emailTaken)
                return Error.Conflict("User.EmailTaken", "Ya existe un usuario con ese email.");
        }

        // Check phone uniqueness (excluding self)
        if (request.Phone is not null)
        {
            var phoneTaken = await _db.Users
                .AnyAsync(u => u.TenantId == tenantId
                    && u.Phone == request.Phone
                    && u.Id != request.UserId, cancellationToken);

            if (phoneTaken)
                return Error.Conflict("User.PhoneTaken", "Ya existe un usuario con ese teléfono.");
        }

        user.UpdateRole(request.Role);
        user.UpdateContact(request.Email, request.Phone);

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}
