using MediatR;
using SchoolERP.Application.Documents.Dtos;

namespace SchoolERP.Application.Documents.Commands.CreateDocumentRequirement;

public record CreateDocumentRequirementCommand(
    Guid SchoolId,
    string Name,
    bool IsRequired,
    string? Description,
    string? AcceptedFileTypes,
    int DisplayOrder
) : IRequest<DocumentRequirementDto>;
