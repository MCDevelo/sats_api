using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Enrollments.Commands.TransferStudent;

public class TransferStudentCommandHandler : IRequestHandler<TransferStudentCommand, ErrorOr<Guid>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public TransferStudentCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Guid>> Handle(TransferStudentCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var enrollment = await _db.Enrollments
            .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId && e.TenantId == tenantId, cancellationToken);

        if (enrollment is null)
            return Error.NotFound("Enrollment.NotFound", "Matrícula no encontrada.");

        if (enrollment.Status != EnrollmentStatus.Active)
            return Error.Conflict("Enrollment.NotActive",
                $"La matrícula no está activa (estado actual: {enrollment.Status}).");

        if (enrollment.SectionId == request.TargetSectionId)
            return Error.Conflict("Transfer.SameSection", "El estudiante ya está en esa sección.");

        // Validate target section
        var targetSection = await _db.Sections
            .FirstOrDefaultAsync(s => s.Id == request.TargetSectionId
                && s.TenantId == tenantId
                && s.IsActive, cancellationToken);

        if (targetSection is null)
            return Error.NotFound("Section.NotFound", "Sección destino no encontrada o inactiva.");

        // Sections must be in the same academic year
        if (targetSection.AcademicYearId != enrollment.AcademicYearId)
            return Error.Validation("Transfer.DifferentYear",
                "Solo se puede transferir dentro del mismo año académico.");

        // Check target section capacity
        var targetCount = await _db.Enrollments
            .CountAsync(e => e.SectionId == request.TargetSectionId
                && e.Status == EnrollmentStatus.Active, cancellationToken);

        if (targetCount >= targetSection.Capacity)
            return Error.Conflict("Section.CapacityReached",
                $"La sección destino ha alcanzado su capacidad máxima de {targetSection.Capacity} estudiantes.");

        // Mark current enrollment as transferred
        enrollment.Transfer();

        // Create new enrollment in target section
        var newEnrollment = Enrollment.Create(
            tenantId,
            enrollment.StudentId,
            request.TargetSectionId,
            enrollment.AcademicYearId);

        _db.Enrollments.Add(newEnrollment);
        await _db.SaveChangesAsync(cancellationToken);

        return newEnrollment.Id;
    }
}
