using ErrorOr;
using MediatR;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.GradeLevels.Commands.CreateGradeLevel;

public record CreateGradeLevelCommand(
    Guid SchoolId,
    string Name,
    int Order,
    EducationLevel EducationLevel) : IRequest<ErrorOr<Guid>>;
