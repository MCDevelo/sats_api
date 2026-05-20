using ErrorOr;
using MediatR;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Communications.Commands.CreateAnnouncement;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Communications.Queries.GetAnnouncements;

public record GetAnnouncementsQuery : IRequest<ErrorOr<PagedResult<AnnouncementResult>>>
{
    public Guid SchoolId { get; init; }
    public AnnouncementAudience? Audience { get; init; }
    public Guid? AudienceId { get; init; }
    public bool? IsPublished { get; init; }
    public AnnouncementPriority? Priority { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
