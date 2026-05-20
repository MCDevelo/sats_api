using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Notifications.Commands.MarkAsRead;

public record MarkAsReadCommand(Guid NotificationId) : IRequest<ErrorOr<Success>>;
