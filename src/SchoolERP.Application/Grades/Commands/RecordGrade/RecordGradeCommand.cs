using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Grades.Commands.RecordGrade;

public record RecordGradeCommand(
    Guid EvaluationPlanId,
    IReadOnlyList<GradeEntry> Entries) : IRequest<ErrorOr<RecordGradeResult>>;

public record GradeEntry(
    Guid StudentId,
    decimal Score,
    string? Comments = null);

public record RecordGradeResult(
    Guid EvaluationPlanId,
    string EvaluationName,
    int TotalGraded,
    decimal AverageScore,
    int Passing,
    int Failing,
    IReadOnlyList<GradeEntryResult> Entries);

public record GradeEntryResult(
    Guid GradeEntryId,
    Guid StudentId,
    string StudentFullName,
    decimal Score,
    string LetterGrade,
    bool IsPassing,
    string? Comments);
