using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(
    string Token,
    string? DeviceInfo = null,
    string? IpAddress = null) : IRequest<ErrorOr<RefreshTokenResult>>;

public record RefreshTokenResult(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt);
