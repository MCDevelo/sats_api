using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Documents.Commands.DeleteDocumentRequirement;

public record DeleteDocumentRequirementCommand(Guid Id) : IRequest;

public class DeleteDocumentRequirementCommandHandler : IRequestHandler<DeleteDocumentRequirementCommand>
{
    private readonly IApplicationDbContext _db;
    public DeleteDocumentRequirementCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task Handle(DeleteDocumentRequirementCommand request, CancellationToken cancellationToken)
    {
        var req = await _db.DocumentRequirements.FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"DocumentRequirement {request.Id} not found");
        req.Deactivate();
        await _db.SaveChangesAsync(cancellationToken);
    }
}
