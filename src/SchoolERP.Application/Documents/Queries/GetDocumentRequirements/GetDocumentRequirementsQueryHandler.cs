using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Documents.Dtos;

namespace SchoolERP.Application.Documents.Queries.GetDocumentRequirements;

public class GetDocumentRequirementsQueryHandler : IRequestHandler<GetDocumentRequirementsQuery, List<DocumentRequirementDto>>
{
    private readonly IApplicationDbContext _db;

    public GetDocumentRequirementsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<List<DocumentRequirementDto>> Handle(GetDocumentRequirementsQuery request, CancellationToken cancellationToken)
    {
        return await _db.DocumentRequirements
            .Where(d => d.SchoolId == request.SchoolId && d.IsActive)
            .OrderBy(d => d.DisplayOrder).ThenBy(d => d.Name)
            .Select(d => new DocumentRequirementDto(d.Id, d.SchoolId, d.Name, d.Description,
                d.IsRequired, d.AcceptedFileTypes, d.DisplayOrder, d.IsActive))
            .ToListAsync(cancellationToken);
    }
}
