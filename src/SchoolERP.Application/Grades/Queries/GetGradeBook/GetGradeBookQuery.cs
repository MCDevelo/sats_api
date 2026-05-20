using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Grades.Queries.GetGradeBook;

/// <summary>
/// Libro de calificaciones de una materia para una sección y período.
/// Vista principal del docente para ingresar y revisar notas.
/// </summary>
public record GetGradeBookQuery(
    Guid SubjectId,
    Guid SectionId,
    Guid AcademicPeriodId) : IRequest<ErrorOr<GradeBookResult>>;

public record GradeBookResult(
    Guid SubjectId,
    string SubjectName,
    Guid SectionId,
    string SectionName,
    string GradeLevel,
    string PeriodName,
    bool GradesPublished,
    decimal TotalWeight,
    IReadOnlyList<EvaluationColumn> Evaluations,
    IReadOnlyList<StudentGradeRow> Students);

public record EvaluationColumn(
    Guid PlanId,
    string Name,
    decimal Weight,
    string? Description,
    DateTime? DueDate,
    bool IsPublished);

public record StudentGradeRow(
    Guid StudentId,
    string FullName,
    string? StudentCode,
    IReadOnlyList<GradeCell> Grades,
    decimal? WeightedAverage,
    string? FinalLetterGrade,
    bool IsPassing);

public record GradeCell(
    Guid PlanId,
    Guid? GradeEntryId,
    decimal? Score,
    string? LetterGrade,
    string? Comments,
    bool IsPublished);
