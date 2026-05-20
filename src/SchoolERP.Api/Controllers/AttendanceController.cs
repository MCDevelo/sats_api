using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Application.Attendance.Commands.ExcuseAbsence;
using SchoolERP.Application.Attendance.Commands.RecordAttendance;
using SchoolERP.Application.Attendance.Commands.UpdateAttendanceRecord;
using SchoolERP.Application.Attendance.Queries.GetAttendanceSummary;
using SchoolERP.Application.Attendance.Queries.GetSectionAttendance;
using SchoolERP.Application.Attendance.Queries.GetStudentAttendance;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Api.Controllers;

[Authorize]
public class AttendanceController : BaseApiController
{
    /// <summary>
    /// Obtener asistencia de una sección en una fecha específica.
    /// Devuelve todos los estudiantes matriculados con su estado (o "NotRecorded" si no se ha pasado lista).
    /// </summary>
    [HttpGet("sections/{sectionId:guid}")]
    public async Task<IActionResult> GetSectionAttendance(
        Guid sectionId,
        [FromQuery] DateOnly date,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetSectionAttendanceQuery(sectionId, date), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtener historial y estadísticas de asistencia de un estudiante en un período.
    /// Incluye nivel de riesgo calculado según el algoritmo de detección.
    /// </summary>
    [HttpGet("students/{studentId:guid}")]
    public async Task<IActionResult> GetStudentAttendance(
        Guid studentId,
        [FromQuery] Guid academicPeriodId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetStudentAttendanceQuery(studentId, academicPeriodId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Resumen de asistencia de una sección en un período completo.
    /// Incluye estudiantes en riesgo y estadísticas diarias.
    /// </summary>
    [HttpGet("sections/{sectionId:guid}/summary")]
    public async Task<IActionResult> GetSectionSummary(
        Guid sectionId,
        [FromQuery] Guid academicPeriodId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAttendanceSummaryQuery(sectionId, academicPeriodId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Registrar asistencia de una sección para una fecha (pase de lista).
    /// Si ya existe un registro para esa fecha, actualiza los estados.
    /// </summary>
    [HttpPost("sections/{sectionId:guid}")]
    public async Task<IActionResult> RecordAttendance(
        Guid sectionId,
        [FromBody] RecordAttendanceRequest request,
        CancellationToken cancellationToken)
    {
        var entries = request.Entries.Select(e => new AttendanceEntry(
            StudentId: e.StudentId,
            Status: e.Status,
            Notes: e.Notes,
            ArrivalTime: e.ArrivalTime)).ToList();

        var command = new RecordAttendanceCommand(
            SectionId: sectionId,
            AcademicPeriodId: request.AcademicPeriodId,
            Date: request.Date,
            Entries: entries);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Corregir un registro individual de asistencia (máximo 7 días de antigüedad).
    /// </summary>
    [HttpPut("records/{recordId:guid}")]
    public async Task<IActionResult> UpdateRecord(
        Guid recordId,
        [FromBody] UpdateRecordRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new UpdateAttendanceRecordCommand(recordId, request.Status, request.Notes),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Excusar una ausencia con motivo justificado.
    /// </summary>
    [HttpPost("records/{recordId:guid}/excuse")]
    public async Task<IActionResult> ExcuseAbsence(
        Guid recordId,
        [FromBody] ExcuseAbsenceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new ExcuseAbsenceCommand(recordId, request.Reason), cancellationToken);
        return HandleResult(result);
    }
}

// Request bodies
public record RecordAttendanceRequest(
    Guid AcademicPeriodId,
    DateOnly Date,
    IReadOnlyList<AttendanceEntryRequest> Entries);

public record AttendanceEntryRequest(
    Guid StudentId,
    AttendanceStatus Status,
    string? Notes = null,
    TimeOnly? ArrivalTime = null);

public record UpdateRecordRequest(
    AttendanceStatus Status,
    string? Notes);

public record ExcuseAbsenceRequest(string Reason);
