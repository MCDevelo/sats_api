using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Documents.Dtos;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Documents.Commands.UploadStudentDocument;

public record UploadStudentDocumentCommand(
    Guid StudentId,
    Guid RequirementId,
    Stream FileStream,
    string FileName,
    string ContentType,
    long FileSizeBytes
) : IRequest<StudentDocumentDto>;

public class UploadStudentDocumentCommandHandler : IRequestHandler<UploadStudentDocumentCommand, StudentDocumentDto>
{
    private readonly IApplicationDbContext _db;
    private readonly IStorageService _storage;
    private readonly ICurrentUserService _currentUser;

    public UploadStudentDocumentCommandHandler(IApplicationDbContext db, IStorageService storage, ICurrentUserService currentUser)
    {
        _db = db;
        _storage = storage;
        _currentUser = currentUser;
    }

    public async Task<StudentDocumentDto> Handle(UploadStudentDocumentCommand request, CancellationToken cancellationToken)
    {
        var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == request.StudentId, cancellationToken)
            ?? throw new KeyNotFoundException($"Student {request.StudentId} not found");

        var requirement = await _db.DocumentRequirements.FirstOrDefaultAsync(r => r.Id == request.RequirementId, cancellationToken)
            ?? throw new KeyNotFoundException($"DocumentRequirement {request.RequirementId} not found");

        var subPath = $"{_currentUser.TenantId}/{request.StudentId}";
        var fileUrl = await _storage.UploadAsync(request.FileStream, request.FileName, request.ContentType, subPath, cancellationToken);

        // Replace existing document for same requirement if exists
        var existing = await _db.StudentDocuments
            .FirstOrDefaultAsync(d => d.StudentId == request.StudentId && d.RequirementId == request.RequirementId, cancellationToken);

        if (existing is not null)
        {
            await _storage.DeleteAsync(existing.FileUrl, cancellationToken);
            _db.StudentDocuments.Remove(existing);
        }

        var doc = StudentDocument.Create(_currentUser.TenantId!.Value, request.StudentId, request.RequirementId,
            request.FileName, fileUrl, request.FileSizeBytes, request.ContentType);

        _db.StudentDocuments.Add(doc);
        await _db.SaveChangesAsync(cancellationToken);

        return new StudentDocumentDto(doc.Id, doc.StudentId, doc.RequirementId,
            requirement.Name, requirement.IsRequired,
            doc.FileName, doc.FileUrl, doc.FileSizeBytes, doc.ContentType,
            doc.Status.ToString(), doc.Notes, doc.VerifiedAt, doc.CreatedAt);
    }
}
