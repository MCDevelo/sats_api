using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Communications.Commands.PublishAnnouncement;

public record PublishAnnouncementCommand(Guid AnnouncementId) : IRequest<ErrorOr<Success>>;
