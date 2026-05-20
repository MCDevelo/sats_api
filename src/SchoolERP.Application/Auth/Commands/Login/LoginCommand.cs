using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Auth.Commands.Login;

public record LoginCommand(
    string EmailOrPhone,
    string Password,
    string? DeviceInfo = null,
    string? IpAddress = null) : IRequest<ErrorOr<LoginResult>>;

public record LoginResult(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserInfo User);

public record UserInfo(
    Guid Id,
    Guid TenantId,
    string? Email,
    string? Phone,
    string Role,
    string PreferredLanguage,
    string? AvatarUrl);
