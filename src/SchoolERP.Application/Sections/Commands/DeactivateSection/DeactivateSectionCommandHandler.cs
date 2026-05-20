using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Sections.Commands.DeactivateSection;

public class DeactivateSectionCommandHandler : IRequestHandler<DeactivateSectionCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeactivateSectionCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(DeactivateSectionCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var section = await _db.Sections
            .FirstOrDefaultAsync(s => s.Id == request.SectionId && s.TenantId == tenantId, cancellationToken);

        if (section is null)
            return Error.NotFound("Section.NotFound", "Sección no encontrada.");

        if (!section.IsActive)
            return Error.Conflict("Section.AlreadyInactive", "La sección ya está desactivada.");

        var activeEnrollments = await _db.Enrollments
            .AnyAsync(e => e.SectionId == request.SectionId && e.Status == EnrollmentStatus.Active, cancellationToken);

        if (activeEnrollments)
            return Error.Conflict("Section.HasActiveEnrollments",
                "No se puede desactivar una sección con estudiantes matriculados activos.");

        section.Deactivate();
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}
