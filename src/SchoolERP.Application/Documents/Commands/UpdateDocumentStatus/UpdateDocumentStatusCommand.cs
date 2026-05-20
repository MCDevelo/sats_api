using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Documents.Dtos;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Documents.Commands.UpdateDocumentStatus;

public record UpdateDocumentStatusCommand(Guid DocumentId, string Status, string? Notes) : IRequest<StudentDocumentDto>;

public class UpdateDocumentStatusCommandHandler : IRequestHandler<UpdateDocumentStatusCommand, StudentDocumentDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateDocumentStatusCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<StudentDocumentDto> Handle(UpdateDocumentStatusCommand request, CancellationToken cancellationToken)
    {
        var doc = await _db.StudentDocuments
            .Include(d => d.Requirement)
            .FirstOrDefaultAsync(d => d.Id == request.DocumentId, cancellationToken)
            ?? throw new KeyNotFoundException($"StudentDocument {request.DocumentId} not found");

        if (Enum.Parse<DocumentStatus>(request.Status) == DocumentStatus.Verified)
            doc.Verify(_currentUser.UserId!.Value);
        else
            doc.Reject(request.Notes, _currentUser.UserId!.Value);

        await _db.SaveChangesAsync(cancellationToken);

        return new StudentDocumentDto(doc.Id, doc.StudentId, doc.RequirementId,
            doc.Requirement.Name, doc.Requirement.IsRequired,
            doc.FileName, doc.FileUrl, doc.FileSizeBytes, doc.ContentType,
            doc.Status.ToString(), doc.Notes, doc.VerifiedAt, doc.CreatedAt);
    }
}
