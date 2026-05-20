using MediatR;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Documents.Dtos;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Documents.Commands.CreateDocumentRequirement;

public class CreateDocumentRequirementCommandHandler : IRequestHandler<CreateDocumentRequirementCommand, DocumentRequirementDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateDocumentRequirementCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<DocumentRequirementDto> Handle(CreateDocumentRequirementCommand request, CancellationToken cancellationToken)
    {
        var req = DocumentRequirement.Create(
            _currentUser.TenantId!.Value, request.SchoolId, request.Name,
            request.IsRequired, request.Description, request.AcceptedFileTypes, request.DisplayOrder);

        _db.DocumentRequirements.Add(req);
        await _db.SaveChangesAsync(cancellationToken);

        return new DocumentRequirementDto(req.Id, req.SchoolId, req.Name, req.Description,
            req.IsRequired, req.AcceptedFileTypes, req.DisplayOrder, req.IsActive);
    }
}
