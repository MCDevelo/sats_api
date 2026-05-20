using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Documents.Dtos;

namespace SchoolERP.Application.Documents.Queries.GetStudentDocuments;

public class GetStudentDocumentsQueryHandler : IRequestHandler<GetStudentDocumentsQuery, List<StudentDocumentDto>>
{
    private readonly IApplicationDbContext _db;

    public GetStudentDocumentsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<List<StudentDocumentDto>> Handle(GetStudentDocumentsQuery request, CancellationToken cancellationToken)
    {
        return await _db.StudentDocuments
            .Include(d => d.Requirement)
            .Where(d => d.StudentId == request.StudentId)
            .OrderBy(d => d.Requirement.DisplayOrder)
            .Select(d => new StudentDocumentDto(
                d.Id, d.StudentId, d.RequirementId,
                d.Requirement.Name, d.Requirement.IsRequired,
                d.FileName, d.FileUrl, d.FileSizeBytes, d.ContentType,
                d.Status.ToString(), d.Notes, d.VerifiedAt, d.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
