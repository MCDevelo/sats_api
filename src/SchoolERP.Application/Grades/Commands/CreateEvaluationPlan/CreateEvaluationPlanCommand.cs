using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Grades.Commands.CreateEvaluationPlan;

public record CreateEvaluationPlanCommand(
    Guid SubjectId,
    Guid AcademicPeriodId,
    string Name,
    decimal Weight,
    string? Description = null,
    DateTime? DueDate = null) : IRequest<ErrorOr<EvaluationPlanResult>>;

public record EvaluationPlanResult(
    Guid Id,
    Guid SubjectId,
    string SubjectName,
    Guid AcademicPeriodId,
    string PeriodName,
    string Name,
    decimal Weight,
    string? Description,
    DateTime? DueDate,
    bool IsPublished,
    DateTime CreatedAt);
