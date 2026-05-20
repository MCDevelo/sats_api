using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Sections.Commands.CreateSection;

public class CreateSectionCommandHandler : IRequestHandler<CreateSectionCommand, ErrorOr<Guid>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateSectionCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var gradeLevel = await _db.GradeLevels
            .FirstOrDefaultAsync(g => g.Id == request.GradeLevelId
                && g.SchoolId == request.SchoolId
                && g.TenantId == tenantId
                && g.IsActive, cancellationToken);

        if (gradeLevel is null)
            return Error.NotFound("GradeLevel.NotFound", "Nivel de grado no encontrado.");

        var yearExists = await _db.AcademicYears
            .AnyAsync(y => y.Id == request.AcademicYearId
                && y.SchoolId == request.SchoolId
                && y.TenantId == tenantId, cancellationToken);

        if (!yearExists)
            return Error.NotFound("AcademicYear.NotFound", "Año académico no encontrado.");

        var nameExists = await _db.Sections
            .AnyAsync(s => s.GradeLevelId == request.GradeLevelId
                && s.AcademicYearId == request.AcademicYearId
                && s.Name == request.Name
                && s.Shift == request.Shift, cancellationToken);

        if (nameExists)
            return Error.Conflict("Section.AlreadyExists",
                $"Ya existe la sección '{request.Name}' en turno {request.Shift} para este nivel y año.");

        var section = Section.Create(
            tenantId,
            request.SchoolId,
            request.GradeLevelId,
            request.AcademicYearId,
            request.Name,
            request.Shift,
            request.Capacity,
            request.Classroom);

        _db.Sections.Add(section);
        await _db.SaveChangesAsync(cancellationToken);

        return section.Id;
    }
}
