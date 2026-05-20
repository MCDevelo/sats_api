using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Application.Reports.Queries.GetAttendanceReport;
using SchoolERP.Application.Reports.Queries.GetEnrollmentReport;
using SchoolERP.Application.Reports.Queries.GetFinancialReport;
using SchoolERP.Application.Reports.Queries.GetReportCardPdf;

namespace SchoolERP.Api.Controllers;

[Authorize]
public class ReportsController : BaseApiController
{
    /// <summary>
    /// Reporte de asistencia de una sección para un período académico.
    /// Devuelve un PDF con la lista de estudiantes y sus estadísticas de asistencia.
    /// </summary>
    [HttpGet("attendance")]
    public async Task<IActionResult> AttendanceReport(
        [FromQuery] Guid sectionId,
        [FromQuery] Guid academicPeriodId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetAttendanceReportQuery(sectionId, academicPeriodId),
            cancellationToken);

        return result.IsError
            ? HandleResult(result)
            : File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    /// <summary>
    /// Boletín de calificaciones individual de un estudiante para un período académico.
    /// Devuelve un PDF con notas por materia, promedio ponderado, puesto y estado de promoción.
    /// </summary>
    [HttpGet("report-card")]
    public async Task<IActionResult> ReportCard(
        [FromQuery] Guid studentId,
        [FromQuery] Guid academicPeriodId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetReportCardPdfQuery(studentId, academicPeriodId),
            cancellationToken);

        return result.IsError
            ? HandleResult(result)
            : File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    /// <summary>
    /// Reporte financiero del centro para un rango de fechas.
    /// Incluye KPIs, desglose mensual y métodos de pago.
    /// </summary>
    [HttpGet("financial")]
    public async Task<IActionResult> FinancialReport(
        [FromQuery] Guid schoolId,
        [FromQuery] DateOnly dateFrom,
        [FromQuery] DateOnly dateTo,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetFinancialReportQuery(schoolId, dateFrom, dateTo),
            cancellationToken);

        return result.IsError
            ? HandleResult(result)
            : File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    /// <summary>
    /// Reporte de matrícula por nivel educativo y sección para un año académico.
    /// Incluye totales por género.
    /// </summary>
    [HttpGet("enrollment")]
    public async Task<IActionResult> EnrollmentReport(
        [FromQuery] Guid schoolId,
        [FromQuery] Guid academicYearId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetEnrollmentReportQuery(schoolId, academicYearId),
            cancellationToken);

        return result.IsError
            ? HandleResult(result)
            : File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }
}
