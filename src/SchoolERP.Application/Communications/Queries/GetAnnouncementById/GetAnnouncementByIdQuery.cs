using ErrorOr;
using MediatR;
using SchoolERP.Application.Communications.Commands.CreateAnnouncement;

namespace SchoolERP.Application.Communications.Queries.GetAnnouncementById;

public record GetAnnouncementByIdQuery(Guid AnnouncementId) : IRequest<ErrorOr<AnnouncementResult>>;
