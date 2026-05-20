using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Application.Finance.Commands.ApplyDiscount;
using SchoolERP.Application.Finance.Commands.CancelInvoice;
using SchoolERP.Application.Finance.Commands.CreateInvoice;
using SchoolERP.Application.Finance.Commands.GenerateBulkInvoices;
using SchoolERP.Application.Finance.Commands.RecordPayment;
using SchoolERP.Application.Finance.Queries.GetFinanceSummary;
using SchoolERP.Application.Finance.Queries.GetInvoiceById;
using SchoolERP.Application.Finance.Queries.GetOverdueInvoices;
using SchoolERP.Application.Finance.Queries.GetStudentInvoices;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Api.Controllers;

[Authorize]
public class FinanceController : BaseApiController
{
    // ── Invoices ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Crear una factura individual para un estudiante.
    /// </summary>
    [HttpPost("invoices")]
    public async Task<IActionResult> CreateInvoice(
        [FromBody] CreateInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateInvoiceCommand(
            StudentId: request.StudentId,
            AcademicYearId: request.AcademicYearId,
            Description: request.Description,
            Amount: request.Amount,
            DueDate: request.DueDate,
            Month: request.Month,
            Year: request.Year,
            Discount: request.Discount,
            Ncf: request.Ncf,
            Notes: request.Notes);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Generar facturas en lote para todos los estudiantes activos de una escuela.
    /// Omite estudiantes que ya tengan factura para ese mes/año.
    /// </summary>
    [HttpPost("invoices/bulk")]
    public async Task<IActionResult> GenerateBulkInvoices(
        [FromBody] BulkInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var command = new GenerateBulkInvoicesCommand(
            SchoolId: request.SchoolId,
            AcademicYearId: request.AcademicYearId,
            Month: request.Month,
            Year: request.Year,
            Description: request.Description,
            Amount: request.Amount,
            DueDate: request.DueDate,
            DefaultDiscount: request.DefaultDiscount);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtener factura por ID con historial completo de pagos.
    /// </summary>
    [HttpGet("invoices/{id:guid}")]
    public async Task<IActionResult> GetInvoice(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetInvoiceByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Historial de facturas de un estudiante con filtros por estado y año.
    /// </summary>
    [HttpGet("students/{studentId:guid}/invoices")]
    public async Task<IActionResult> GetStudentInvoices(
        Guid studentId,
        [FromQuery] string? status,
        [FromQuery] int? year,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetStudentInvoicesQuery
        {
            StudentId = studentId,
            Status = status,
            Year = year,
            Page = page,
            PageSize = pageSize
        };

        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Aplicar descuento a una factura pendiente.
    /// </summary>
    [HttpPost("invoices/{id:guid}/discount")]
    public async Task<IActionResult> ApplyDiscount(
        Guid id,
        [FromBody] ApplyDiscountRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new ApplyDiscountCommand(id, request.Discount, request.Reason),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Cancelar una factura sin pagos registrados.
    /// </summary>
    [HttpDelete("invoices/{id:guid}")]
    public async Task<IActionResult> CancelInvoice(
        Guid id,
        [FromBody] CancelInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new CancelInvoiceCommand(id, request.Reason), cancellationToken);
        return HandleResult(result);
    }

    // ── Payments ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Registrar un pago sobre una factura.
    /// Actualiza automáticamente el estado (Partial → Paid cuando el balance llega a 0).
    /// </summary>
    [HttpPost("invoices/{invoiceId:guid}/payments")]
    public async Task<IActionResult> RecordPayment(
        Guid invoiceId,
        [FromBody] RecordPaymentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RecordPaymentCommand(
            InvoiceId: invoiceId,
            Amount: request.Amount,
            Method: request.Method,
            Reference: request.Reference,
            PaidAt: request.PaidAt,
            Notes: request.Notes);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    // ── Reports ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Resumen financiero de una escuela: facturado, cobrado, pendiente, vencido,
    /// desglose mensual y por método de pago.
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(
        [FromQuery] Guid schoolId,
        [FromQuery] Guid academicYearId,
        [FromQuery] int? month,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetFinanceSummaryQuery(schoolId, academicYearId, month),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Lista de facturas vencidas con datos de contacto del tutor para cobros.
    /// </summary>
    [HttpGet("invoices/overdue")]
    public async Task<IActionResult> GetOverdue(
        [FromQuery] Guid? schoolId,
        [FromQuery] Guid? academicYearId,
        [FromQuery] int? daysOverdueMin,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetOverdueInvoicesQuery
        {
            SchoolId = schoolId,
            AcademicYearId = academicYearId,
            DaysOverdueMin = daysOverdueMin,
            Search = search,
            Page = page,
            PageSize = pageSize
        };

        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }
}

// Request bodies
public record CreateInvoiceRequest(
    Guid StudentId,
    Guid AcademicYearId,
    string Description,
    decimal Amount,
    DateTime DueDate,
    int Month,
    int Year,
    decimal Discount = 0,
    string? Ncf = null,
    string? Notes = null);

public record BulkInvoiceRequest(
    Guid SchoolId,
    Guid AcademicYearId,
    int Month,
    int Year,
    string Description,
    decimal Amount,
    DateTime DueDate,
    decimal DefaultDiscount = 0);

public record RecordPaymentRequest(
    decimal Amount,
    PaymentMethod Method,
    string? Reference = null,
    DateTime? PaidAt = null,
    string? Notes = null);

public record ApplyDiscountRequest(decimal Discount, string Reason);

public record CancelInvoiceRequest(string Reason);
