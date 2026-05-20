using ErrorOr;
using MediatR;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Communications.Commands.CreateAnnouncement;

public record CreateAnnouncementCommand(
    Guid SchoolId,
    string Title,
    string Body,
    AnnouncementAudience Audience,
    AnnouncementPriority Priority,
    Guid? AudienceId = null,
    DateTime? ExpiresAt = null,
    bool PublishNow = false) : IRequest<ErrorOr<AnnouncementResult>>;

public record AnnouncementResult(
    Guid Id,
    Guid SchoolId,
    string SchoolName,
    string AuthorName,
    string Title,
    string Body,
    string Audience,
    Guid? AudienceId,
    string Priority,
    bool IsPublished,
    DateTime? PublishedAt,
    DateTime? ExpiresAt,
    DateTime CreatedAt);
