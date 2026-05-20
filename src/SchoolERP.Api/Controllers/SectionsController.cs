using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Application.Enrollments.Commands.EnrollStudent;
using SchoolERP.Application.Enrollments.Commands.TransferStudent;
using SchoolERP.Application.Enrollments.Commands.WithdrawEnrollment;
using SchoolERP.Application.Enrollments.Queries.GetSectionEnrollments;
using SchoolERP.Application.Enrollments.Queries.GetStudentEnrollments;
using SchoolERP.Application.Sections.Commands.AssignHomeTeacher;
using SchoolERP.Application.Sections.Commands.CreateSection;
using SchoolERP.Application.Sections.Commands.DeactivateSection;
using SchoolERP.Application.Sections.Commands.UpdateSection;
using SchoolERP.Application.Sections.Queries.GetSection;
using SchoolERP.Application.Sections.Queries.GetSections;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Api.Controllers;

[Authorize(Roles = "Admin,SchoolAdmin,Director,Secretary,Coordinator")]
public class SectionsController : BaseApiController
{
    // ── Sections ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Lista de secciones. Filtros: schoolId, gradeLevelId, academicYearId, shift, isActive.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetSections(
        [FromQuery] Guid? schoolId = null,
        [FromQuery] Guid? gradeLevelId = null,
        [FromQuery] Guid? academicYearId = null,
        [FromQuery] Shift? shift = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(
            new GetSectionsQuery(schoolId, gradeLevelId, academicYearId, shift, isActive, page, pageSize),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Detalle de una sección con estudiantes y materias asignadas.</summary>
    [HttpGet("{sectionId:guid}")]
    public async Task<IActionResult> GetSection(Guid sectionId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetSectionQuery(sectionId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Crear sección.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,SchoolAdmin,Director")]
    public async Task<IActionResult> CreateSection(
        [FromBody] CreateSectionRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new CreateSectionCommand(
                request.SchoolId,
                request.GradeLevelId,
                request.AcademicYearId,
                request.Name,
                request.Shift,
                request.Capacity,
                request.Classroom),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Actualizar nombre, turno, capacidad y aula de la sección.</summary>
    [HttpPut("{sectionId:guid}")]
    [Authorize(Roles = "Admin,SchoolAdmin,Director")]
    public async Task<IActionResult> UpdateSection(
        Guid sectionId, [FromBody] UpdateSectionRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new UpdateSectionCommand(sectionId, request.Name, request.Shift, request.Capacity, request.Classroom),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Asignar o remover docente guía.
    /// Enviar teacherId = null para remover el docente asignado.
    /// </summary>
    [HttpPatch("{sectionId:guid}/home-teacher")]
    [Authorize(Roles = "Admin,SchoolAdmin,Director")]
    public async Task<IActionResult> AssignHomeTeacher(
        Guid sectionId, [FromBody] AssignHomeTeacherRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new AssignHomeTeacherCommand(sectionId, request.TeacherId),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Desactivar sección (requiere 0 estudiantes activos).</summary>
    [HttpDelete("{sectionId:guid}")]
    [Authorize(Roles = "Admin,SchoolAdmin,Director")]
    public async Task<IActionResult> DeactivateSection(Guid sectionId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeactivateSectionCommand(sectionId), cancellationToken);
        return HandleResult(result);
    }

    // ── Enrollments ────────────────────────────────────────────────────────────

    /// <summary>
    /// Lista de matrículas de una sección.
    /// Por defecto solo muestra activas. Pasar status para ver otras.
    /// </summary>
    [HttpGet("{sectionId:guid}/enrollments")]
    public async Task<IActionResult> GetSectionEnrollments(
        Guid sectionId,
        [FromQuery] EnrollmentStatus? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(
            new GetSectionEnrollmentsQuery(sectionId, status, page, pageSize),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Matricular un estudiante en esta sección.</summary>
    [HttpPost("{sectionId:guid}/enrollments")]
    public async Task<IActionResult> EnrollStudent(
        Guid sectionId, [FromBody] SectionEnrollRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new EnrollStudentCommand(request.StudentId, sectionId, request.EnrollmentDate, request.Notes),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Retirar (dar de baja) una matrícula.</summary>
    [HttpPatch("enrollments/{enrollmentId:guid}/withdraw")]
    public async Task<IActionResult> WithdrawEnrollment(
        Guid enrollmentId, [FromBody] WithdrawEnrollmentRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new WithdrawEnrollmentCommand(enrollmentId, request.Reason),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Transferir un estudiante a otra sección del mismo año académico.
    /// Retorna el ID de la nueva matrícula.
    /// </summary>
    [HttpPost("enrollments/{enrollmentId:guid}/transfer")]
    public async Task<IActionResult> TransferStudent(
        Guid enrollmentId, [FromBody] TransferStudentRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new TransferStudentCommand(enrollmentId, request.TargetSectionId),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Historial de matrículas de un estudiante (todos los años).</summary>
    [HttpGet("students/{studentId:guid}/enrollments")]
    public async Task<IActionResult> GetStudentEnrollments(Guid studentId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetStudentEnrollmentsQuery(studentId), cancellationToken);
        return HandleResult(result);
    }
}

// ── Request body types ─────────────────────────────────────────────────────────

public record CreateSectionRequest(
    Guid SchoolId,
    Guid GradeLevelId,
    Guid AcademicYearId,
    string Name,
    Shift Shift,
    int Capacity = 35,
    string? Classroom = null);

public record UpdateSectionRequest(
    string Name,
    Shift Shift,
    int Capacity,
    string? Classroom);

public record AssignHomeTeacherRequest(Guid? TeacherId);

public record SectionEnrollRequest(
    Guid StudentId,
    DateTime? EnrollmentDate = null,
    string? Notes = null);

public record WithdrawEnrollmentRequest(string Reason);

public record TransferStudentRequest(Guid TargetSectionId);
