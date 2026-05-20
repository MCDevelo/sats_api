using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Application.Teachers.Commands.AssignTeacher;
using SchoolERP.Application.Teachers.Commands.CreateTeacher;
using SchoolERP.Application.Teachers.Commands.DeactivateTeacher;
using SchoolERP.Application.Teachers.Commands.RemoveAssignment;
using SchoolERP.Application.Teachers.Commands.UpdateTeacher;
using SchoolERP.Application.Teachers.Queries.GetSectionAssignments;
using SchoolERP.Application.Teachers.Queries.GetTeacherAssignments;
using SchoolERP.Application.Teachers.Queries.GetTeacherById;
using SchoolERP.Application.Teachers.Queries.GetTeacherWorkload;
using SchoolERP.Application.Teachers.Queries.GetTeachers;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Api.Controllers;

[Authorize]
public class TeachersController : BaseApiController
{
    /// <summary>
    /// Listar docentes con filtros y paginación.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? schoolId,
        [FromQuery] bool? isActive,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTeachersQuery
        {
            SchoolId = schoolId,
            IsActive = isActive ?? true,
            Search = search,
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDesc = sortDesc
        };

        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtener docente por ID con asignaciones actuales.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetTeacherByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtener carga horaria del docente en un año académico.
    /// Si no se especifica academic_year_id, usa el año activo actual.
    /// </summary>
    [HttpGet("{id:guid}/workload")]
    public async Task<IActionResult> GetWorkload(
        Guid id,
        [FromQuery] Guid? academicYearId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new GetTeacherWorkloadQuery(id, academicYearId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Crear nuevo docente.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTeacherRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTeacherCommand(
            SchoolId: request.SchoolId,
            FirstName: request.FirstName,
            LastName: request.LastName,
            ContractType: request.ContractType,
            HireDate: request.HireDate,
            Email: request.Email,
            Phone: request.Phone,
            NationalId: request.NationalId,
            MinerdCode: request.MinerdCode,
            TeacherCode: request.TeacherCode,
            AcademicTitle: request.AcademicTitle,
            Specialization: request.Specialization,
            Qualifications: request.Qualifications,
            Gender: request.Gender,
            DateOfBirth: request.DateOfBirth,
            Address: request.Address,
            ContractEndDate: request.ContractEndDate,
            WorkingHoursPerWeek: request.WorkingHoursPerWeek);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Actualizar datos del docente.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTeacherRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateTeacherCommand(
            TeacherId: id,
            FirstName: request.FirstName,
            LastName: request.LastName,
            Email: request.Email,
            Phone: request.Phone,
            Address: request.Address,
            Specialization: request.Specialization,
            Qualifications: request.Qualifications,
            AcademicTitle: request.AcademicTitle,
            WorkingHoursPerWeek: request.WorkingHoursPerWeek,
            ContractEndDate: request.ContractEndDate);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Desactivar docente.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeactivateTeacherCommand(id), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Historial de asignaciones del docente. Por defecto solo activas.
    /// </summary>
    [HttpGet("{id:guid}/assignments")]
    public async Task<IActionResult> GetAssignments(
        Guid id,
        [FromQuery] Guid? academicYearId = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(
            new GetTeacherAssignmentsQuery(id, academicYearId, isActive),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Asignar docente a materia/sección/año académico.
    /// </summary>
    [HttpPost("{id:guid}/assignments")]
    public async Task<IActionResult> Assign(Guid id, [FromBody] AssignTeacherRequest request, CancellationToken cancellationToken)
    {
        var command = new AssignTeacherCommand(
            TeacherId: id,
            SectionId: request.SectionId,
            SubjectId: request.SubjectId,
            AcademicYearId: request.AcademicYearId);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Remover una asignación (desactivar). No elimina el historial.
    /// </summary>
    [HttpDelete("{id:guid}/assignments/{assignmentId:guid}")]
    public async Task<IActionResult> RemoveAssignment(Guid id, Guid assignmentId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new RemoveAssignmentCommand(assignmentId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Lista de asignaciones activas de una sección (qué docente enseña qué materia).
    /// </summary>
    [HttpGet("sections/{sectionId:guid}/assignments")]
    public async Task<IActionResult> GetSectionAssignments(
        Guid sectionId,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new GetSectionAssignmentsQuery(sectionId, isActive), cancellationToken);
        return HandleResult(result);
    }
}

// ── Request bodies ─────────────────────────────────────────────────────────────

public record CreateTeacherRequest(
    Guid SchoolId,
    string FirstName,
    string LastName,
    ContractType ContractType,
    DateTime HireDate,
    string? Email = null,
    string? Phone = null,
    string? NationalId = null,
    string? MinerdCode = null,
    string? TeacherCode = null,
    string? AcademicTitle = null,
    string? Specialization = null,
    string? Qualifications = null,
    Gender? Gender = null,
    DateTime? DateOfBirth = null,
    string? Address = null,
    DateTime? ContractEndDate = null,
    int WorkingHoursPerWeek = 40);

public record UpdateTeacherRequest(
    string FirstName,
    string LastName,
    string? Email = null,
    string? Phone = null,
    string? Address = null,
    string? Specialization = null,
    string? Qualifications = null,
    string? AcademicTitle = null,
    int WorkingHoursPerWeek = 40,
    DateTime? ContractEndDate = null);

public record AssignTeacherRequest(
    Guid SectionId,
    Guid SubjectId,
    Guid AcademicYearId);
