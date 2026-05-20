using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Teachers.Commands.AssignTeacher;

public class AssignTeacherCommandHandler : IRequestHandler<AssignTeacherCommand, ErrorOr<AssignmentResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public AssignTeacherCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<AssignmentResult>> Handle(AssignTeacherCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var teacher = await _db.Teachers
            .FirstOrDefaultAsync(t => t.Id == request.TeacherId && t.TenantId == tenantId && t.IsActive, cancellationToken);

        if (teacher is null)
            return Error.NotFound(description: "Docente no encontrado.");

        var section = await _db.Sections
            .Include(s => s.GradeLevel)
            .Include(s => s.AcademicYear)
            .FirstOrDefaultAsync(s => s.Id == request.SectionId && s.TenantId == tenantId && s.IsActive, cancellationToken);

        if (section is null)
            return Error.NotFound(description: "Sección no encontrada.");

        var subject = await _db.Subjects
            .FirstOrDefaultAsync(s => s.Id == request.SubjectId && s.TenantId == tenantId && s.IsActive, cancellationToken);

        if (subject is null)
            return Error.NotFound(description: "Materia no encontrada.");

        // Ensure subject belongs to the grade level of the section
        if (subject.GradeLevelId != section.GradeLevelId)
            return Error.Validation(description: "La materia no corresponde al grado de la sección.");

        // Check for duplicate assignment
        var exists = await _db.TeacherAssignments
            .AnyAsync(a =>
                a.TeacherId == request.TeacherId &&
                a.SectionId == request.SectionId &&
                a.SubjectId == request.SubjectId &&
                a.AcademicYearId == request.AcademicYearId &&
                a.IsActive, cancellationToken);

        if (exists)
            return Error.Conflict(description: "El docente ya está asignado a esta materia en esta sección.");

        var assignment = TeacherAssignment.Create(
            teacherId: request.TeacherId,
            sectionId: request.SectionId,
            subjectId: request.SubjectId,
            academicYearId: request.AcademicYearId);

        _db.TeacherAssignments.Add(assignment);
        await _db.SaveChangesAsync(cancellationToken);

        return new AssignmentResult(
            AssignmentId: assignment.Id,
            TeacherId: teacher.Id,
            TeacherFullName: teacher.FullName,
            SectionId: section.Id,
            SectionName: $"{section.GradeLevel.Name} - {section.Name}",
            SubjectId: subject.Id,
            SubjectName: subject.Name,
            AcademicYear: section.AcademicYear.Name);
    }
}
