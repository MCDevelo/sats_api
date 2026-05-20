using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Schools.Commands.UpdateSchool;

public record UpdateSchoolCommand(
    Guid SchoolId,
    string Name,
    string? Email,
    string? PhonePrimary,
    string? Address) : IRequest<ErrorOr<Success>>;
