using ErrorOr;
using MediatR;
using SchoolERP.Application.Guardians.Commands.LinkGuardianToStudent;

namespace SchoolERP.Application.Guardians.Queries.GetStudentGuardians;

public record GetStudentGuardiansQuery(Guid StudentId)
    : IRequest<ErrorOr<List<StudentGuardianResult>>>;
