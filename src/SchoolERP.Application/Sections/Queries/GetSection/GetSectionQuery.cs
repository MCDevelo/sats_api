using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Sections.Queries.GetSection;

public record GetSectionQuery(Guid SectionId) : IRequest<ErrorOr<SectionDetailResult>>;

public record SectionDetailResult(
    Guid Id,
    Guid SchoolId,
    Guid GradeLevelId,
    string GradeLevelName,
    Guid AcademicYearId,
    string AcademicYearName,
    string Name,
    string Shift,
    int Capacity,
    int EnrolledCount,
    Guid? HomeTeacherId,
    string? HomeTeacherName,
    string? Classroom,
    bool IsActive,
    DateTime CreatedAt,
    IReadOnlyList<SectionStudentSummary> Students,
    IReadOnlyList<SectionSubjectSummary> Subjects);

public record SectionStudentSummary(
    Guid EnrollmentId,
    Guid StudentId,
    string FullName,
    string? StudentCode,
    string? Nse,
    string Gender,
    string? PhotoUrl,
    DateTime EnrollmentDate);

public record SectionSubjectSummary(
    Guid AssignmentId,
    Guid SubjectId,
    string SubjectName,
    Guid? TeacherId,
    string? TeacherName);
