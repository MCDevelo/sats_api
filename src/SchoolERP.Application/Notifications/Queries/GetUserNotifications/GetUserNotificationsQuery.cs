using ErrorOr;
using MediatR;
using SchoolERP.Application.Common.Models;

namespace SchoolERP.Application.Notifications.Queries.GetUserNotifications;

public record GetUserNotificationsQuery(
    int Page = 1,
    int PageSize = 20,
    bool? OnlyUnread = null) : IRequest<ErrorOr<PagedResult<NotificationResult>>>;

public record NotificationResult(
    Guid Id,
    string EventType,
    string Title,
    string Body,
    string? Data,
    bool IsRead,
    DateTime? ReadAt,
    DateTime CreatedAt);
