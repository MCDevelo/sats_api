using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Infrastructure.Jobs;

namespace SchoolERP.Api.Controllers;

/// <summary>
/// Admin-only endpoints to trigger or inspect background jobs.
/// </summary>
[Authorize(Roles = "Admin")]
public class JobsController : BaseApiController
{
    private readonly IBackgroundJobClient _jobClient;

    public JobsController(IBackgroundJobClient jobClient)
    {
        _jobClient = jobClient;
    }

    /// <summary>
    /// Genera mensualidades en bloque para todos los estudiantes activos de un centro.
    /// Omite automáticamente a quienes ya tienen factura para el mes/año indicado.
    /// Retorna el ID del job de Hangfire para seguimiento.
    /// </summary>
    [HttpPost("monthly-invoices")]
    public IActionResult EnqueueMonthlyInvoices([FromBody] MonthlyInvoiceJobRequest request)
    {
        var jobId = _jobClient.Enqueue<MonthlyInvoiceGenerationJob>(job =>
            job.ExecuteAsync(
                request.SchoolId,
                request.AcademicYearId,
                request.Month,
                request.Year,
                request.Amount,
                request.Description));

        return Ok(new { JobId = jobId, Status = "Enqueued" });
    }

    /// <summary>
    /// Ejecuta inmediatamente el marcado de facturas vencidas (sin esperar al cron diario).
    /// </summary>
    [HttpPost("mark-overdue-invoices")]
    public IActionResult EnqueueOverdueMarking()
    {
        var jobId = _jobClient.Enqueue<OverdueInvoiceMarkingJob>(job => job.ExecuteAsync());
        return Ok(new { JobId = jobId, Status = "Enqueued" });
    }

    /// <summary>
    /// Ejecuta inmediatamente el reintento de notificaciones fallidas.
    /// </summary>
    [HttpPost("retry-notifications")]
    public IActionResult EnqueueNotificationRetry()
    {
        var jobId = _jobClient.Enqueue<NotificationRetryJob>(job => job.ExecuteAsync());
        return Ok(new { JobId = jobId, Status = "Enqueued" });
    }

    /// <summary>
    /// Ejecuta inmediatamente los recordatorios de asistencia.
    /// </summary>
    [HttpPost("attendance-reminders")]
    public IActionResult EnqueueAttendanceReminders()
    {
        var jobId = _jobClient.Enqueue<AttendanceReminderJob>(job => job.ExecuteAsync());
        return Ok(new { JobId = jobId, Status = "Enqueued" });
    }
}

public record MonthlyInvoiceJobRequest(
    Guid SchoolId,
    Guid AcademicYearId,
    int Month,
    int Year,
    decimal Amount,
    string Description);
