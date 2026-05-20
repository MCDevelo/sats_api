using ErrorOr;
using MediatR;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Teachers.Queries.GetTeacherAssignments;

public record GetTeacherAssignmentsQuery(
    Guid TeacherId,
    Guid? AcademicYearId = null,
    bool? IsActive = null) : IRequest<ErrorOr<List<TeacherAssignmentResult>>>;

public record TeacherAssignmentResult(
    Guid AssignmentId,
    Guid TeacherId,
    string TeacherFullName,
    Guid SectionId,
    string SectionName,
    Shift SectionShift,
    Guid GradeLevelId,
    string GradeLevelName,
    Guid SubjectId,
    string SubjectName,
    Guid AcademicYearId,
    string AcademicYearName,
    bool IsActive,
    DateTime CreatedAt);
