using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Application.Grades.Commands.CreateEvaluationPlan;
using SchoolERP.Application.Grades.Commands.PublishGrades;
using SchoolERP.Application.Grades.Commands.RecordGrade;
using SchoolERP.Application.Grades.Commands.UpdateGrade;
using SchoolERP.Application.Grades.Queries.GetGradeBook;
using SchoolERP.Application.Grades.Queries.GetStudentReportCard;
using SchoolERP.Application.Grades.Queries.GetSubjectStats;

namespace SchoolERP.Api.Controllers;

[Authorize]
public class GradesController : BaseApiController
{
    // ── Evaluation Plans ─────────────────────────────────────────────────────

    /// <summary>
    /// Crear un ítem de evaluación (prueba, tarea, examen) para una materia/período.
    /// La suma de pesos no puede exceder 100%.
    /// </summary>
    [HttpPost("evaluation-plans")]
    public async Task<IActionResult> CreateEvaluationPlan(
        [FromBody] CreateEvaluationPlanRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateEvaluationPlanCommand(
            SubjectId: request.SubjectId,
            AcademicPeriodId: request.AcademicPeriodId,
            Name: request.Name,
            Weight: request.Weight,
            Description: request.Description,
            DueDate: request.DueDate);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    // ── Grade Entry ───────────────────────────────────────────────────────────

    /// <summary>
    /// Registrar calificaciones en bloque para una evaluación.
    /// Hace upsert: crea o actualiza según exista ya un registro.
    /// </summary>
    [HttpPost("evaluation-plans/{planId:guid}/grades")]
    public async Task<IActionResult> RecordGrades(
        Guid planId,
        [FromBody] RecordGradesRequest request,
        CancellationToken cancellationToken)
    {
        var entries = request.Entries
            .Select(e => new Application.Grades.Commands.RecordGrade.GradeEntry(e.StudentId, e.Score, e.Comments))
            .ToList();

        var result = await Mediator.Send(new RecordGradeCommand(planId, entries), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Corregir una calificación individual (no publicada).
    /// </summary>
    [HttpPut("entries/{gradeEntryId:guid}")]
    public async Task<IActionResult> UpdateGrade(
        Guid gradeEntryId,
        [FromBody] UpdateGradeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new UpdateGradeCommand(gradeEntryId, request.Score, request.Comments),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Publicar todas las calificaciones de un período.
    /// Una vez publicadas no pueden modificarse.
    /// </summary>
    [HttpPost("periods/{periodId:guid}/publish")]
    public async Task<IActionResult> PublishGrades(
        Guid periodId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new PublishGradesCommand(periodId), cancellationToken);
        return HandleResult(result);
    }

    // ── Queries ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Libro de calificaciones (vista docente): columnas por evaluación, filas por estudiante.
    /// </summary>
    [HttpGet("grade-book")]
    public async Task<IActionResult> GetGradeBook(
        [FromQuery] Guid subjectId,
        [FromQuery] Guid sectionId,
        [FromQuery] Guid academicPeriodId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetGradeBookQuery(subjectId, sectionId, academicPeriodId),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Libreta de calificaciones del estudiante: promedio ponderado por materia,
    /// promedio general, ranking y estado de promoción.
    /// </summary>
    [HttpGet("report-card")]
    public async Task<IActionResult> GetReportCard(
        [FromQuery] Guid studentId,
        [FromQuery] Guid academicPeriodId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetStudentReportCardQuery(studentId, academicPeriodId),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Estadísticas de rendimiento por materia y sección:
    /// promedio, distribución por rango, tasa de aprobación por evaluación.
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetSubjectStats(
        [FromQuery] Guid subjectId,
        [FromQuery] Guid sectionId,
        [FromQuery] Guid academicPeriodId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetSubjectStatsQuery(subjectId, sectionId, academicPeriodId),
            cancellationToken);

        return HandleResult(result);
    }
}

// Request bodies
public record CreateEvaluationPlanRequest(
    Guid SubjectId,
    Guid AcademicPeriodId,
    string Name,
    decimal Weight,
    string? Description = null,
    DateTime? DueDate = null);

public record RecordGradesRequest(IReadOnlyList<GradeEntryRequest> Entries);

public record GradeEntryRequest(
    Guid StudentId,
    decimal Score,
    string? Comments = null);

public record UpdateGradeRequest(decimal Score, string? Comments);
