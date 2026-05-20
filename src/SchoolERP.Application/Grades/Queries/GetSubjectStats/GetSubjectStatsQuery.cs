using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Grades.Queries.GetSubjectStats;

/// <summary>
/// Estadísticas de rendimiento por materia para una sección y período.
/// </summary>
public record GetSubjectStatsQuery(
    Guid SubjectId,
    Guid SectionId,
    Guid AcademicPeriodId) : IRequest<ErrorOr<SubjectStatsResult>>;

public record SubjectStatsResult(
    string SubjectName,
    string SectionName,
    string PeriodName,
    int TotalStudents,
    int Graded,
    decimal? ClassAverage,
    decimal? HighestScore,
    decimal? LowestScore,
    int Passing,
    int Failing,
    decimal PassRate,
    IReadOnlyList<ScoreDistribution> Distribution,
    IReadOnlyList<EvaluationStat> EvaluationStats);

public record ScoreDistribution(string Range, int Count, decimal Percentage);

public record EvaluationStat(
    string EvaluationName,
    decimal Weight,
    decimal? Average,
    decimal? Highest,
    decimal? Lowest,
    int Graded);
