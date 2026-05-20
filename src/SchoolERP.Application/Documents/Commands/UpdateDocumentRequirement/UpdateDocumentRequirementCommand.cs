using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Documents.Dtos;

namespace SchoolERP.Application.Documents.Commands.UpdateDocumentRequirement;

public record UpdateDocumentRequirementCommand(Guid Id, string Name, bool IsRequired, string? Description, string? AcceptedFileTypes, int DisplayOrder) : IRequest<DocumentRequirementDto>;

public class UpdateDocumentRequirementCommandHandler : IRequestHandler<UpdateDocumentRequirementCommand, DocumentRequirementDto>
{
    private readonly IApplicationDbContext _db;
    public UpdateDocumentRequirementCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<DocumentRequirementDto> Handle(UpdateDocumentRequirementCommand request, CancellationToken cancellationToken)
    {
        var req = await _db.DocumentRequirements.FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"DocumentRequirement {request.Id} not found");

        req.Update(request.Name, request.IsRequired, request.Description, request.AcceptedFileTypes, request.DisplayOrder);
        await _db.SaveChangesAsync(cancellationToken);

        return new DocumentRequirementDto(req.Id, req.SchoolId, req.Name, req.Description,
            req.IsRequired, req.AcceptedFileTypes, req.DisplayOrder, req.IsActive);
    }
}
