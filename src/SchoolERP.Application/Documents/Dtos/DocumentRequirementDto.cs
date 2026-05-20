namespace SchoolERP.Application.Documents.Dtos;

public record DocumentRequirementDto(
    Guid Id,
    Guid SchoolId,
    string Name,
    string? Description,
    bool IsRequired,
    string AcceptedFileTypes,
    int DisplayOrder,
    bool IsActive
);

public record StudentDocumentDto(
    Guid Id,
    Guid StudentId,
    Guid RequirementId,
    string RequirementName,
    bool RequirementIsRequired,
    string FileName,
    string FileUrl,
    long FileSizeBytes,
    string ContentType,
    string Status,
    string? Notes,
    DateTime? VerifiedAt,
    DateTime CreatedAt
);
