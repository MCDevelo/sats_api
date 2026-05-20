using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Notifications.Queries.GetUnreadCount;

public record GetUnreadCountQuery : IRequest<ErrorOr<int>>;
