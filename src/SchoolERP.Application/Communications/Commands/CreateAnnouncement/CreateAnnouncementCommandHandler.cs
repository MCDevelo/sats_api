using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Communications.Commands.CreateAnnouncement;

public class CreateAnnouncementCommandHandler
    : IRequestHandler<CreateAnnouncementCommand, ErrorOr<AnnouncementResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateAnnouncementCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<AnnouncementResult>> Handle(
        CreateAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;
        var authorId = _currentUser.UserId!.Value;

        var school = await _db.Schools
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.SchoolId && s.TenantId == tenantId && s.IsActive, cancellationToken);

        if (school is null)
            return Error.NotFound("School.NotFound", "Escuela no encontrada.");

        // Validate AudienceId exists when required
        if (request.Audience == AnnouncementAudience.Section && request.AudienceId.HasValue)
        {
            var sectionExists = await _db.Sections
                .AnyAsync(s => s.Id == request.AudienceId.Value && s.TenantId == tenantId, cancellationToken);

            if (!sectionExists)
                return Error.NotFound("Section.NotFound", "Sección no encontrada.");
        }

        if (request.Audience == AnnouncementAudience.GradeLevel && request.AudienceId.HasValue)
        {
            var gradeLevelExists = await _db.GradeLevels
                .AnyAsync(g => g.Id == request.AudienceId.Value && g.SchoolId == request.SchoolId, cancellationToken);

            if (!gradeLevelExists)
                return Error.NotFound("GradeLevel.NotFound", "Nivel educativo no encontrado.");
        }

        var author = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == authorId, cancellationToken);

        var announcement = Announcement.Create(
            tenantId: tenantId,
            schoolId: request.SchoolId,
            authorId: authorId,
            title: request.Title,
            body: request.Body,
            audience: request.Audience,
            priority: request.Priority,
            audienceId: request.AudienceId,
            expiresAt: request.ExpiresAt);

        if (request.PublishNow)
            announcement.Publish();

        _db.Announcements.Add(announcement);
        await _db.SaveChangesAsync(cancellationToken);

        return new AnnouncementResult(
            Id: announcement.Id,
            SchoolId: school.Id,
            SchoolName: school.Name,
            AuthorName: author?.Email ?? string.Empty,
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
