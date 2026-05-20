using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ErrorOr<Guid>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IPasswordService _passwordService;

    public CreateUserCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        IPasswordService passwordService)
    {
        _db = db;
        _currentUser = currentUser;
        _passwordService = passwordService;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        if (request.Email is not null)
        {
            var emailExists = await _db.Users
                .AnyAsync(u => u.TenantId == tenantId && u.Email == request.Email.ToLowerInvariant(), cancellationToken);

            if (emailExists)
                return Error.Conflict("User.EmailTaken", "Ya existe un usuario con ese email en este tenant.");
        }

        if (request.Phone is not null)
        {
            var phoneExists = await _db.Users
                .AnyAsync(u => u.TenantId == tenantId && u.Phone == request.Phone, cancellationToken);

            if (phoneExists)
                return Error.Conflict("User.PhoneTaken", "Ya existe un usuario con ese teléfono en este tenant.");
        }

        var hash = _passwordService.Hash(request.Password);

        var user = User.Create(tenantId, request.Role, request.Email, request.Phone, hash);

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
