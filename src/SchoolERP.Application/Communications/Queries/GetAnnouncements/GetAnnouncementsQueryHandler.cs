using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Communications.Commands.CreateAnnouncement;

namespace SchoolERP.Application.Communications.Queries.GetAnnouncements;

public class GetAnnouncementsQueryHandler
    : IRequestHandler<GetAnnouncementsQuery, ErrorOr<PagedResult<AnnouncementResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetAnnouncementsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<PagedResult<AnnouncementResult>>> Handle(
        GetAnnouncementsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var schoolExists = await _db.Schools
            .AnyAsync(s => s.Id == request.SchoolId && s.TenantId == tenantId, cancellationToken);

        if (!schoolExists)
            return Error.NotFound("School.NotFound", "Escuela no encontrada.");

        var query = _db.Announcements
            .AsNoTracking()
            .Include(a => a.School)
            .Include(a => a.Author)
            .Where(a => a.SchoolId == request.SchoolId && a.TenantId == tenantId);

        if (request.Audience.HasValue)
            query = query.Where(a => a.Audience == request.Audience.Value);

        if (request.AudienceId.HasValue)
            query = query.Where(a => a.AudienceId == request.AudienceId.Value);

        if (request.IsPublished.HasValue)
            query = query.Where(a => a.IsPublished == request.IsPublished.Value);

        if (request.Priority.HasValue)
            query = query.Where(a => a.Priority == request.Priority.Value);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.Priority)
            .ThenByDescending(a => a.PublishedAt ?? a.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new AnnouncementResult(
                a.Id,
                a.SchoolId,
                a.School.Name,
                a.Author.Email ?? string.Empty,
                a.Title,
                a.Body,
                a.Audience.ToString(),
                a.AudienceId,
                a.Priority.ToString(),
                a.IsPublished,
                a.PublishedAt,
                a.ExpiresAt,
                a.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<AnnouncementResult>(items, total, request.Page, request.PageSize);
    }
}
