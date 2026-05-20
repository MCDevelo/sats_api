using ErrorOr;
using MediatR;
using SchoolERP.Application.Common.Models;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Users.Queries.GetUsers;

public record GetUsersQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    UserRole? Role = null,
    bool? IsActive = null) : IRequest<ErrorOr<PagedResult<UserResult>>>;

public record UserResult(
    Guid Id,
    string? Email,
    string? Phone,
    UserRole Role,
    bool IsActive,
    bool EmailVerified,
    bool PhoneVerified,
    DateTime? LastLogin,
    bool IsLockedOut,
    DateTime? LockedUntil,
    string? AvatarUrl,
    DateTime CreatedAt);
