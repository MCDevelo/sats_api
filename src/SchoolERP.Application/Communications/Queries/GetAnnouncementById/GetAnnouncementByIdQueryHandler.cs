using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Communications.Commands.CreateAnnouncement;

namespace SchoolERP.Application.Communications.Queries.GetAnnouncementById;

public class GetAnnouncementByIdQueryHandler
    : IRequestHandler<GetAnnouncementByIdQuery, ErrorOr<AnnouncementResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetAnnouncementByIdQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<AnnouncementResult>> Handle(
        GetAnnouncementByIdQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var announcement = await _db.Announcements
            .AsNoTracking()
            .Include(a => a.School)
            .Include(a => a.Author)
            .FirstOrDefaultAsync(a => a.Id == request.AnnouncementId && a.TenantId == tenantId, cancellationToken);

        if (announcement is null)
            return Error.NotFound("Announcement.NotFound", "Comunicado no encontrado.");

        return new AnnouncementResult(
            Id: announcement.Id,
            SchoolId: announcement.SchoolId,
            SchoolName: announcement.School.Name,
            AuthorName: announcement.Author.Email ?? string.Empty,
            Title: announcement.Title,
            Body: announcement.Body,
            Audience: announcement.Audience.ToString(),
            AudienceId: announcement.AudienceId,
            Priority: announcement.Priority.ToString(),
            IsPublished: announcement.IsPublished,
            PublishedAt: announcement.PublishedAt,
            ExpiresAt: announcement.ExpiresAt,
            CreatedAt: announcement.CreatedAt);
    }
}
