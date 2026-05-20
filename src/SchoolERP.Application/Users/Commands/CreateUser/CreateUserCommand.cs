using ErrorOr;
using MediatR;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Users.Commands.CreateUser;

public record CreateUserCommand(
    UserRole Role,
    string Password,
    string? Email = null,
    string? Phone = null) : IRequest<ErrorOr<Guid>>;
