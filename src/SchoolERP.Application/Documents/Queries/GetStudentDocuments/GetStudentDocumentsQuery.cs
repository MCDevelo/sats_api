using MediatR;
using SchoolERP.Application.Documents.Dtos;

namespace SchoolERP.Application.Documents.Queries.GetStudentDocuments;

public record GetStudentDocumentsQuery(Guid StudentId) : IRequest<List<StudentDocumentDto>>;
