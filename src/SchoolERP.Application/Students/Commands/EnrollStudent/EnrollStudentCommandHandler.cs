using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Students.Commands.EnrollStudent;

public class EnrollStudentCommandHandler : IRequestHandler<EnrollStudentCommand, ErrorOr<EnrollmentResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public EnrollStudentCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<EnrollmentResult>> Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var student = await _db.Students
            .FirstOrDefaultAsync(s => s.Id == request.StudentId && s.TenantId == tenantId && s.IsActive, cancellationToken);

        if (student is null)
            return Error.NotFound(description: "Estudiante no encontrado.");

        var section = await _db.Sections
            .Include(s => s.GradeLevel)
            .Include(s => s.AcademicYear)
            .FirstOrDefaultAsync(s => s.Id == request.SectionId && s.TenantId == tenantId && s.IsActive, cancellationToken);

        if (section is null)
            return Error.NotFound(description: "Sección no encontrada.");

        if (section.AcademicYearId != request.AcademicYearId)
            return Error.Validation(description: "La sección no pertenece al año académico indicado.");

        // Check if student is already actively enrolled in this academic year
        var existingEnrollment = await _db.Enrollments
            .FirstOrDefaultAsync(e =>
                e.StudentId == request.StudentId &&
                e.AcademicYearId == request.AcademicYearId &&
                e.Status == EnrollmentStatus.Active, cancellationToken);

        if (existingEnrollment is not null)
            return Error.Conflict(description: "El estudiante ya tiene una matrícula activa en este año académico.");

        // Check section capacity
        var enrolledCount = await _db.Enrollments
            .CountAsync(e => e.SectionId == request.SectionId && e.Status == EnrollmentStatus.Active, cancellationToken);

        if (enrolledCount >= section.Capacity)
            return Error.Conflict(description: $"La sección ha alcanzado su capacidad máxima de {section.Capacity} estudiantes.");

        var enrollment = Enrollment.Create(
            tenantId: tenantId,
            studentId: request.StudentId,
            sectionId: request.SectionId,
            academicYearId: request.AcademicYearId,
            enrollmentDate: request.EnrollmentDate);

        _db.Enrollments.Add(enrollment);
        await _db.SaveChangesAsync(cancellationToken);

        return new EnrollmentResult(
            EnrollmentId: enrollment.Id,
            StudentId: student.Id,
            StudentFullName: student.FullName,
            SectionId: section.Id,
            SectionName: $"{section.GradeLevel.Name} - {section.Name}",
            AcademicYearId: section.AcademicYear.Id,
            AcademicYearName: section.AcademicYear.Name,
            Status: enrollment.Status.ToString(),
            EnrollmentDate: enrollment.EnrollmentDate);
    }
}
