using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Sections.Commands.UpdateSection;

public class UpdateSectionCommandHandler : IRequestHandler<UpdateSectionCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateSectionCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var section = await _db.Sections
            .FirstOrDefaultAsync(s => s.Id == request.SectionId && s.TenantId == tenantId, cancellationToken);

        if (section is null)
            return Error.NotFound("Section.NotFound", "Sección no encontrada.");

        // Validate new capacity >= current active enrollment count
        var activeCount = await _db.Enrollments
            .CountAsync(e => e.SectionId == request.SectionId
                && e.Status == Domain.Enums.EnrollmentStatus.Active, cancellationToken);

        if (request.Capacity < activeCount)
            return Error.Validation("Section.CapacityTooLow",
                $"La capacidad ({request.Capacity}) no puede ser menor al número de estudiantes matriculados ({activeCount}).");

        // Check name+shift uniqueness (excluding self)
        var conflict = await _db.Sections
            .AnyAsync(s => s.GradeLevelId == section.GradeLevelId
                && s.AcademicYearId == section.AcademicYearId
                && s.Name == request.Name
                && s.Shift == request.Shift
                && s.Id != request.SectionId, cancellationToken);

        if (conflict)
            return Error.Conflict("Section.AlreadyExists",
                $"Ya existe la sección '{request.Name}' en turno {request.Shift} para este nivel y año.");

        section.Update(request.Name, request.Shift, request.Capacity, request.Classroom);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}
