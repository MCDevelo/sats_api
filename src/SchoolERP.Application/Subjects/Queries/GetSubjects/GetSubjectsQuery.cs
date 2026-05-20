using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Subjects.Queries.GetSubjects;

public record GetSubjectsQuery(
    Guid GradeLevelId,
    bool? IsActive = null) : IRequest<ErrorOr<List<SubjectResult>>>;

public record SubjectResult(
    Guid Id,
    Guid GradeLevelId,
    Guid SchoolId,
    string Name,
    string? Code,
    string? Description,
    int WeeklyHours,
    bool IsRequired,
    bool IsActive,
    DateTime CreatedAt);
