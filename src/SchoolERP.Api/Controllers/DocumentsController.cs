using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Application.Documents.Commands.CreateDocumentRequirement;
using SchoolERP.Application.Documents.Commands.DeleteDocumentRequirement;
using SchoolERP.Application.Documents.Commands.UpdateDocumentRequirement;
using SchoolERP.Application.Documents.Commands.UpdateDocumentStatus;
using SchoolERP.Application.Documents.Commands.UploadStudentDocument;
using SchoolERP.Application.Documents.Queries.GetDocumentRequirements;
using SchoolERP.Application.Documents.Queries.GetStudentDocuments;

namespace SchoolERP.Api.Controllers;

[Authorize]
public class DocumentsController : BaseApiController
{
    // ── Requirements ──────────────────────────────────────────────────

    [HttpGet("requirements")]
    public async Task<IActionResult> GetRequirements([FromQuery] Guid schoolId, CancellationToken ct)
    {
        var result = await Mediator.Send(new GetDocumentRequirementsQuery(schoolId), ct);
        return Ok(new { success = true, data = result });
    }

    [HttpPost("requirements")]
    [Authorize(Roles = "PlatformAdmin,TenantAdmin,SchoolAdmin,Director")]
    public async Task<IActionResult> CreateRequirement([FromBody] CreateDocumentRequirementCommand command, CancellationToken ct)
    {
        var result = await Mediator.Send(command, ct);
        return StatusCode(201, new { success = true, data = result });
    }

    [HttpPut("requirements/{id:guid}")]
    [Authorize(Roles = "PlatformAdmin,TenantAdmin,SchoolAdmin,Director")]
    public async Task<IActionResult> UpdateRequirement(Guid id, [FromBody] UpdateDocumentRequirementCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest();
        var result = await Mediator.Send(command, ct);
        return Ok(new { success = true, data = result });
    }

    [HttpDelete("requirements/{id:guid}")]
    [Authorize(Roles = "PlatformAdmin,TenantAdmin,SchoolAdmin,Director")]
    public async Task<IActionResult> DeleteRequirement(Guid id, CancellationToken ct)
    {
        await Mediator.Send(new DeleteDocumentRequirementCommand(id), ct);
        return NoContent();
    }

    // ── Student Documents ─────────────────────────────────────────────

    [HttpGet("students/{studentId:guid}")]
    public async Task<IActionResult> GetStudentDocuments(Guid studentId, CancellationToken ct)
    {
        var result = await Mediator.Send(new GetStudentDocumentsQuery(studentId), ct);
        return Ok(new { success = true, data = result });
    }

    [HttpPost("students/{studentId:guid}")]
    [Authorize(Roles = "PlatformAdmin,TenantAdmin,SchoolAdmin,Director,Secretary")]
    public async Task<IActionResult> UploadDocument(Guid studentId, [FromForm] Guid requirementId, IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0) return BadRequest("No file provided.");

        var command = new UploadStudentDocumentCommand(
            studentId, requirementId,
            file.OpenReadStream(), file.FileName, file.ContentType, file.Length);

        var result = await Mediator.Send(command, ct);
        return StatusCode(201, new { success = true, data = result });
    }

    [HttpPatch("{documentId:guid}/status")]
    [Authorize(Roles = "PlatformAdmin,TenantAdmin,SchoolAdmin,Director")]
    public async Task<IActionResult> UpdateStatus(Guid documentId, [FromBody] UpdateDocumentStatusRequest request, CancellationToken ct)
    {
        var result = await Mediator.Send(new UpdateDocumentStatusCommand(documentId, request.Status, request.Notes), ct);
        return Ok(new { success = true, data = result });
    }
}

public record UpdateDocumentStatusRequest(string Status, string? Notes);
