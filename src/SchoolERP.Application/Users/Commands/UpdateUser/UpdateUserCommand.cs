using ErrorOr;
using MediatR;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid UserId,
    UserRole Role,
    string? Email = null,
    string? Phone = null) : IRequest<ErrorOr<Success>>;
