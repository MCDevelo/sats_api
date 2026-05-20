using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

public class StudentDocument : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid RequirementId { get; private set; }
    public string FileName { get; private set; } = default!;
    public string FileUrl { get; private set; } = default!;
    public long FileSizeBytes { get; private set; }
    public string ContentType { get; private set; } = default!;
    public DocumentStatus Status { get; private set; } = DocumentStatus.Received;
    public string? Notes { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public Guid? VerifiedByUserId { get; private set; }

    // Navigation
    public Student Student { get; private set; } = default!;
    public DocumentRequirement Requirement { get; private set; } = default!;

    private StudentDocument() { }

    public static StudentDocument Create(Guid tenantId, Guid studentId, Guid requirementId,
        string fileName, string fileUrl, long fileSizeBytes, string contentType)
    {
        return new StudentDocument
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            StudentId = studentId,
            RequirementId = requirementId,
            FileName = fileName,
            FileUrl = fileUrl,
            FileSizeBytes = fileSizeBytes,
            ContentType = contentType,
            Status = DocumentStatus.Received,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Verify(Guid verifiedBy)
    {
        Status = DocumentStatus.Verified;
        VerifiedAt = DateTime.UtcNow;
        VerifiedByUserId = verifiedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(string? notes, Guid rejectedBy)
    {
        Status = DocumentStatus.Rejected;
        Notes = notes;
        VerifiedByUserId = rejectedBy;
        UpdatedAt = DateTime.UtcNow;
    }
}
