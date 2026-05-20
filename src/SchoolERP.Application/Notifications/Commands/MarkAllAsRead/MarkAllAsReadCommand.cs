using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Notifications.Commands.MarkAllAsRead;

public record MarkAllAsReadCommand : IRequest<ErrorOr<int>>;
