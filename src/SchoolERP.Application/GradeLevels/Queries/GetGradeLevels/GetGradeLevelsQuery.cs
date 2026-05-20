using ErrorOr;
using MediatR;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.GradeLevels.Queries.GetGradeLevels;

public record GetGradeLevelsQuery(Guid SchoolId) : IRequest<ErrorOr<List<GradeLevelResult>>>;

public record GradeLevelResult(
    Guid Id,
    Guid SchoolId,
    string Name,
    int Order,
    EducationLevel EducationLevel,
    bool IsActive,
    int SubjectCount,
    int SectionCount,
    DateTime CreatedAt);
