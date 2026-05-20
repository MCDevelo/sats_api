using MediatR;
using SchoolERP.Application.Documents.Dtos;

namespace SchoolERP.Application.Documents.Queries.GetDocumentRequirements;

public record GetDocumentRequirementsQuery(Guid SchoolId) : IRequest<List<DocumentRequirementDto>>;
