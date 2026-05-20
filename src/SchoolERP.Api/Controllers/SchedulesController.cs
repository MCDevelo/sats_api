using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Application.Scheduling.Commands.CreateScheduleSlot;
using SchoolERP.Application.Scheduling.Commands.DeleteScheduleSlot;
using SchoolERP.Application.Scheduling.Commands.UpdateScheduleSlot;
using SchoolERP.Application.Scheduling.Queries.GetSectionSchedule;
using SchoolERP.Application.Scheduling.Queries.GetTeacherSchedule;

namespace SchoolERP.Api.Controllers;

[Authorize]
public class SchedulesController : BaseApiController
{
    // ── Queries ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Horario semanal de una sección: franjas ordenadas por día y hora,
    /// con materia, docente y aula.
    /// </summary>
    [HttpGet("sections/{sectionId:guid}")]
    public async Task<IActionResult> GetSectionSchedule(
        Guid sectionId,
        [FromQuery] Guid academicYearId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetSectionScheduleQuery(sectionId, academicYearId),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Horario semanal de un docente: todas sus franjas across secciones.
    /// </summary>
    [HttpGet("teachers/{teacherId:guid}")]
    public async Task<IActionResult> GetTeacherSchedule(
        Guid teacherId,
        [FromQuery] Guid academicYearId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetTeacherScheduleQuery(teacherId, academicYearId),
            cancellationToken);

        return HandleResult(result);
    }

    // ── Commands ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Agregar una franja horaria a una asignación docente.
    /// Detecta conflictos de docente y sección automáticamente.
    /// </summary>
    [HttpPost("slots")]
    public async Task<IActionResult> CreateSlot(
        [FromBody] CreateScheduleSlotRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new CreateScheduleSlotCommand(
                request.TeacherAssignmentId,
                request.Day,
                request.StartTime,
                request.EndTime,
                request.Room),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Modificar día u horario de una franja existente.
    /// Re-valida conflictos excluyendo la franja actual.
    /// </summary>
    [HttpPut("slots/{slotId:guid}")]
    public async Task<IActionResult> UpdateSlot(
        Guid slotId,
        [FromBody] UpdateScheduleSlotRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new UpdateScheduleSlotCommand(
                slotId,
                request.Day,
                request.StartTime,
                request.EndTime,
                request.Room),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Eliminar una franja horaria.
    /// </summary>
    [HttpDelete("slots/{slotId:guid}")]
    public async Task<IActionResult> DeleteSlot(
        Guid slotId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeleteScheduleSlotCommand(slotId), cancellationToken);
        return HandleResult(result);
    }
}

// Request bodies
public record CreateScheduleSlotRequest(
    Guid TeacherAssignmentId,
    DayOfWeek Day,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string? Room = null);

public record UpdateScheduleSlotRequest(
    DayOfWeek Day,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string? Room = null);
