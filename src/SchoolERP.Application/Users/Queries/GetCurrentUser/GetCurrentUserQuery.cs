using ErrorOr;
using MediatR;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Users.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<ErrorOr<CurrentUserResult>>;

public record CurrentUserResult(
    Guid Id,
    Guid TenantId,
    string? Email,
    string? Phone,
    UserRole Role,
    bool IsActive,
    bool EmailVerified,
    bool PhoneVerified,
    DateTime? LastLogin,
    string? AvatarUrl,
    string PreferredLanguage,
    bool TwoFactorEnabled,
    DateTime CreatedAt);
