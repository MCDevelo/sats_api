using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Communications.Commands.PublishAnnouncement;

public class PublishAnnouncementCommandHandler
    : IRequestHandler<PublishAnnouncementCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public PublishAnnouncementCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(
        PublishAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var announcement = await _db.Announcements
            .FirstOrDefaultAsync(a => a.Id == request.AnnouncementId && a.TenantId == tenantId, cancellationToken);

        if (announcement is null)
            return Error.NotFound("Announcement.NotFound", "Comunicado no encontrado.");

        if (announcement.IsPublished)
            return Error.Conflict("Announcement.AlreadyPublished", "El comunicado ya está publicado.");

        announcement.Publish();
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
