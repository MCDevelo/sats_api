using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Teachers.Commands.AssignTeacher;

public record AssignTeacherCommand(
    Guid TeacherId,
    Guid SectionId,
    Guid SubjectId,
    Guid AcademicYearId) : IRequest<ErrorOr<AssignmentResult>>;

public record AssignmentResult(
    Guid AssignmentId,
    Guid TeacherId,
    string TeacherFullName,
    Guid SectionId,
    string SectionName,
    Guid SubjectId,
    string SubjectName,
    string AcademicYear);
