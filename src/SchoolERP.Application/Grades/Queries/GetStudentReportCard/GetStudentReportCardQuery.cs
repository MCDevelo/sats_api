using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Grades.Queries.GetStudentReportCard;

/// <summary>
/// Libreta de calificaciones completa del estudiante para un período.
/// Vista del tutor/estudiante.
/// </summary>
public record GetStudentReportCardQuery(
    Guid StudentId,
    Guid AcademicPeriodId) : IRequest<ErrorOr<ReportCardResult>>;

public record ReportCardResult(
    Guid StudentId,
    string StudentFullName,
    string? StudentCode,
    string SchoolName,
    string GradeLevel,
    string Section,
    string AcademicYear,
    string Period,
    bool IsPublished,
    IReadOnlyList<SubjectGrade> Subjects,
    decimal? GeneralAverage,
    string? GeneralLetterGrade,
    bool IsPromoted,
    int Rank);

public record SubjectGrade(
    Guid SubjectId,
    string SubjectName,
    int WeeklyHours,
    IReadOnlyList<EvaluationGrade> Evaluations,
    decimal? FinalGrade,
    string? LetterGrade,
    bool IsPassing);

public record EvaluationGrade(
    string EvaluationName,
    decimal Weight,
    decimal? Score,
    string? LetterGrade);
