using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Application.Guardians.Commands.CreateGuardian;
using SchoolERP.Application.Guardians.Commands.LinkGuardianToStudent;
using SchoolERP.Application.Guardians.Commands.LinkUserToGuardian;
using SchoolERP.Application.Guardians.Commands.RemoveStudentGuardian;
using SchoolERP.Application.Guardians.Commands.UpdateGuardian;
using SchoolERP.Application.Guardians.Commands.UpdateStudentGuardian;
using SchoolERP.Application.Guardians.Queries.GetGuardianById;
using SchoolERP.Application.Guardians.Queries.GetGuardians;
using SchoolERP.Application.Guardians.Queries.GetStudentGuardians;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Api.Controllers;

[Authorize]
public class GuardiansController : BaseApiController
{
    // ── Guardians CRUD ────────────────────────────────────────────────────────

    /// <summary>
    /// Listar encargados del tenant con búsqueda y filtros.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] bool? hasPortalAccount = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetGuardiansQuery
        {
            Search = search,
            HasPortalAccount = hasPortalAccount,
            Page = page,
            PageSize = pageSize
        };

        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtener encargado por ID con lista de estudiantes vinculados.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetGuardianByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Crear nuevo encargado. Requiere al menos email o teléfono.
    /// La cédula debe ser única dentro del tenant.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateGuardianRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateGuardianCommand(
            FirstName: request.FirstName,
            LastName: request.LastName,
            NationalId: request.NationalId,
            Email: request.Email,
            Phone: request.Phone,
            PhoneSecondary: request.PhoneSecondary,
            WhatsApp: request.WhatsApp,
            Address: request.Address,
            Occupation: request.Occupation,
            Workplace: request.Workplace,
            IsFinancialResponsible: request.IsFinancialResponsible,
            Gender: request.Gender);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Actualizar información del encargado.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateGuardianRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateGuardianCommand(
            GuardianId: id,
            FirstName: request.FirstName,
            LastName: request.LastName,
            Email: request.Email,
            Phone: request.Phone,
            PhoneSecondary: request.PhoneSecondary,
            WhatsApp: request.WhatsApp,
            Address: request.Address,
            Occupation: request.Occupation,
            Workplace: request.Workplace,
            IsFinancialResponsible: request.IsFinancialResponsible);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Vincular cuenta de usuario portal al encargado.
    /// Un usuario solo puede estar vinculado a un encargado por tenant.
    /// </summary>
    [HttpPost("{id:guid}/link-user")]
    public async Task<IActionResult> LinkUser(
        Guid id,
        [FromBody] LinkUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new LinkUserToGuardianCommand(id, request.UserId),
            cancellationToken);

        return HandleResult(result);
    }

    // ── Student ↔ Guardian links ──────────────────────────────────────────────

    /// <summary>
    /// Obtener todos los encargados de un estudiante.
    /// El encargado primario aparece primero en la lista.
    /// </summary>
    [HttpGet("students/{studentId:guid}")]
    public async Task<IActionResult> GetStudentGuardians(
        Guid studentId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetStudentGuardiansQuery(studentId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Vincular un encargado existente a un estudiante.
    /// Relaciones válidas: padre | madre | tutor | abuelo | tio | hermano | otro.
    /// Si IsPrimary = true, el encargado primario anterior pierde ese rol automáticamente.
    /// </summary>
    [HttpPost("students/{studentId:guid}/link")]
    public async Task<IActionResult> LinkToStudent(
        Guid studentId,
        [FromBody] LinkGuardianRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LinkGuardianToStudentCommand(
            StudentId: studentId,
            GuardianId: request.GuardianId,
            Relationship: request.Relationship,
            IsPrimary: request.IsPrimary,
            CanPickup: request.CanPickup,
            IsEmergencyContact: request.IsEmergencyContact,
            ReceivesNotifications: request.ReceivesNotifications,
            HasCustodyOrder: request.HasCustodyOrder,
            CustodyNotes: request.CustodyNotes);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Actualizar los permisos y relación de un vínculo encargado-estudiante.
    /// </summary>
    [HttpPut("links/{linkId:guid}")]
    public async Task<IActionResult> UpdateLink(
        Guid linkId,
        [FromBody] UpdateLinkRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateStudentGuardianCommand(
            StudentGuardianId: linkId,
            Relationship: request.Relationship,
            IsPrimary: request.IsPrimary,
            CanPickup: request.CanPickup,
            IsEmergencyContact: request.IsEmergencyContact,
            ReceivesNotifications: request.ReceivesNotifications,
            HasCustodyOrder: request.HasCustodyOrder,
            CustodyNotes: request.CustodyNotes);

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Desvincular un encargado de un estudiante.
    /// No se puede eliminar el único encargado registrado.
    /// </summary>
    [HttpDelete("links/{linkId:guid}")]
    public async Task<IActionResult> RemoveLink(Guid linkId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new RemoveStudentGuardianCommand(linkId), cancellationToken);
        return HandleResult(result);
    }
}

// ── Request bodies ────────────────────────────────────────────────────────────

public record CreateGuardianRequest(
    string FirstName,
    string LastName,
    string? NationalId = null,
    string? Email = null,
    string? Phone = null,
    string? PhoneSecondary = null,
    string? WhatsApp = null,
    string? Address = null,
    string? Occupation = null,
    string? Workplace = null,
    bool IsFinancialResponsible = false,
    Gender? Gender = null);

public record UpdateGuardianRequest(
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    string? PhoneSecondary,
    string? WhatsApp,
    string? Address,
    string? Occupation,
    string? Workplace,
    bool IsFinancialResponsible);

public record LinkUserRequest(Guid UserId);

public record LinkGuardianRequest(
    Guid GuardianId,
    string Relationship,
    bool IsPrimary = false,
    bool CanPickup = true,
    bool IsEmergencyContact = false,
    bool ReceivesNotifications = true,
    bool HasCustodyOrder = false,
    string? CustodyNotes = null);

public record UpdateLinkRequest(
    string Relationship,
    bool IsPrimary,
    bool CanPickup,
    bool IsEmergencyContact,
    bool ReceivesNotifications,
    bool HasCustodyOrder,
    string? CustodyNotes = null);
