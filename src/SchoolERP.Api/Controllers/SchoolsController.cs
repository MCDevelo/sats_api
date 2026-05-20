using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Application.AcademicPeriods.Commands.CreateAcademicPeriod;
using SchoolERP.Application.AcademicPeriods.Commands.PublishPeriodGrades;
using SchoolERP.Application.AcademicPeriods.Queries.GetAcademicPeriods;
using SchoolERP.Application.AcademicYears.Commands.CreateAcademicYear;
using SchoolERP.Application.AcademicYears.Commands.SetCurrentAcademicYear;
using SchoolERP.Application.AcademicYears.Queries.GetAcademicYears;
using SchoolERP.Application.GradeLevels.Commands.CreateGradeLevel;
using SchoolERP.Application.GradeLevels.Queries.GetGradeLevels;
using SchoolERP.Application.Schools.Commands.CreateSchool;
using SchoolERP.Application.Schools.Commands.DeactivateSchool;
using SchoolERP.Application.Schools.Commands.UpdateSchool;
using SchoolERP.Application.Schools.Queries.GetSchools;
using SchoolERP.Application.Subjects.Commands.CreateSubject;
using SchoolERP.Application.Subjects.Queries.GetSubjects;

namespace SchoolERP.Api.Controllers;

[Authorize(Roles = "PlatformAdmin,TenantAdmin")]
public class SchoolsController : BaseApiController
{
    // ── Schools ───────────────────────────────────────────────────────────────

    /// <summary>Lista de escuelas del tenant actual.</summary>
    [HttpGet]
    public async Task<IActionResult> GetSchools(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new GetSchoolsQuery(page, pageSize, search, isActive), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Crear escuela (PlatformAdmin solo).</summary>
    [HttpPost]
    [Authorize(Roles = "PlatformAdmin")]
    public async Task<IActionResult> CreateSchool(CreateSchoolCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Actualizar datos de la escuela.</summary>
    [HttpPut("{schoolId:guid}")]
    public async Task<IActionResult> UpdateSchool(Guid schoolId, UpdateSchoolCommand command, CancellationToken cancellationToken)
    {
        if (schoolId != command.SchoolId) return BadRequest();
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Desactivar escuela.</summary>
    [HttpDelete("{schoolId:guid}")]
    [Authorize(Roles = "PlatformAdmin")]
    public async Task<IActionResult> DeactivateSchool(Guid schoolId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeactivateSchoolCommand(schoolId), cancellationToken);
        return HandleResult(result);
    }

    // ── Academic Years ─────────────────────────────────────────────────────────

    /// <summary>Lista de años académicos de una escuela.</summary>
    [HttpGet("{schoolId:guid}/academic-years")]
    public async Task<IActionResult> GetAcademicYears(Guid schoolId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAcademicYearsQuery(schoolId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Crear año académico.</summary>
    [HttpPost("{schoolId:guid}/academic-years")]
    public async Task<IActionResult> CreateAcademicYear(
        Guid schoolId,
        CreateAcademicYearCommand command,
        CancellationToken cancellationToken)
    {
        if (schoolId != command.SchoolId) return BadRequest();
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Marcar año académico como el actual (desactiva los demás).</summary>
    [HttpPatch("academic-years/{academicYearId:guid}/set-current")]
    public async Task<IActionResult> SetCurrentAcademicYear(Guid academicYearId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new SetCurrentAcademicYearCommand(academicYearId), cancellationToken);
        return HandleResult(result);
    }

    // ── Academic Periods ───────────────────────────────────────────────────────

    /// <summary>Lista de períodos académicos de un año.</summary>
    [HttpGet("academic-years/{academicYearId:guid}/periods")]
    public async Task<IActionResult> GetAcademicPeriods(Guid academicYearId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAcademicPeriodsQuery(academicYearId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Crear período académico.</summary>
    [HttpPost("academic-years/{academicYearId:guid}/periods")]
    public async Task<IActionResult> CreateAcademicPeriod(
        Guid academicYearId,
        CreateAcademicPeriodCommand command,
        CancellationToken cancellationToken)
    {
        if (academicYearId != command.AcademicYearId) return BadRequest();
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Publicar notas de un período (los estudiantes pueden ver sus calificaciones).</summary>
    [HttpPatch("academic-periods/{periodId:guid}/publish-grades")]
    public async Task<IActionResult> PublishPeriodGrades(Guid periodId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new PublishPeriodGradesCommand(periodId), cancellationToken);
        return HandleResult(result);
    }

    // ── Grade Levels ───────────────────────────────────────────────────────────

    /// <summary>Lista de niveles de grado de una escuela.</summary>
    [HttpGet("{schoolId:guid}/grade-levels")]
    public async Task<IActionResult> GetGradeLevels(Guid schoolId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetGradeLevelsQuery(schoolId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Crear nivel de grado.</summary>
    [HttpPost("{schoolId:guid}/grade-levels")]
    public async Task<IActionResult> CreateGradeLevel(
        Guid schoolId,
        CreateGradeLevelCommand command,
        CancellationToken cancellationToken)
    {
        if (schoolId != command.SchoolId) return BadRequest();
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    // ── Subjects ───────────────────────────────────────────────────────────────

    /// <summary>Lista de materias de un nivel de grado.</summary>
    [HttpGet("grade-levels/{gradeLevelId:guid}/subjects")]
    public async Task<IActionResult> GetSubjects(
        Guid gradeLevelId,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new GetSubjectsQuery(gradeLevelId, isActive), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Crear materia en un nivel de grado.</summary>
    [HttpPost("grade-levels/{gradeLevelId:guid}/subjects")]
    public async Task<IActionResult> CreateSubject(
        Guid gradeLevelId,
        CreateSubjectCommand command,
        CancellationToken cancellationToken)
    {
        if (gradeLevelId != command.GradeLevelId) return BadRequest();
        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
