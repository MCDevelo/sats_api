using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Schools.Commands.DeactivateSchool;

public record DeactivateSchoolCommand(Guid SchoolId) : IRequest<ErrorOr<Success>>;
