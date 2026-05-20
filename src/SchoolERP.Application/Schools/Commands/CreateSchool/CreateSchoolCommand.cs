using ErrorOr;
using MediatR;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Schools.Commands.CreateSchool;

public record CreateSchoolCommand(
    Guid TenantId,
    string Name,
    EducationLevel LevelType,
    string? CodeMinerd = null,
    string? Email = null,
    string? PhonePrimary = null,
    string? Province = null,
    string? Municipality = null,
    string? Address = null,
    string? Rnc = null,
    string? LegalName = null) : IRequest<ErrorOr<Guid>>;
