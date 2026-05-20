using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Enrollments.Commands.EnrollStudent;

public class EnrollStudentCommandHandler : IRequestHandler<EnrollStudentCommand, ErrorOr<Guid>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public EnrollStudentCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Guid>> Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        // Validate section exists and is active
        var section = await _db.Sections
            .FirstOrDefaultAsync(s => s.Id == request.SectionId && s.TenantId == tenantId && s.IsActive, cancellationToken);

        if (section is null)
            return Error.NotFound("Section.NotFound", "Sección no encontrada o inactiva.");

        // Validate student belongs to the same tenant
        var studentExists = await _db.Students
            .AnyAsync(s => s.Id == request.StudentId && s.TenantId == tenantId, cancellationToken);

        if (!studentExists)
            return Error.NotFound("Student.NotFound", "Estudiante no encontrado.");

        // Check if student is already actively enrolled in ANY section for this academic year
        var alreadyEnrolled = await _db.Enrollments
            .AnyAsync(e => e.StudentId == request.StudentId
                && e.AcademicYearId == section.AcademicYearId
                && e.Status == EnrollmentStatus.Active, cancellationToken);

        if (alreadyEnrolled)
            return Error.Conflict("Enrollment.AlreadyEnrolled",
                "El estudiante ya tiene una matrícula activa en este año académico.");

        // Check section capacity
        var enrolledCount = await _db.Enrollments
            .CountAsync(e => e.SectionId == request.SectionId && e.Status == EnrollmentStatus.Active, cancellationToken);

        if (enrolledCount >= section.Capacity)
            return Error.Conflict("Section.CapacityReached",
                $"La sección ha alcanzado su capacidad máxima de {section.Capacity} estudiantes.");

        var enrollment = Enrollment.Create(
            tenantId,
            request.StudentId,
            request.SectionId,
            section.AcademicYearId,
            request.EnrollmentDate);

        _db.Enrollments.Add(enrollment);
        await _db.SaveChangesAsync(cancellationToken);

        return enrollment.Id;
    }
}
