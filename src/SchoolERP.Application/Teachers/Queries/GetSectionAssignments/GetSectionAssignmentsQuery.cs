using ErrorOr;
using MediatR;
using SchoolERP.Application.Teachers.Queries.GetTeacherAssignments;

namespace SchoolERP.Application.Teachers.Queries.GetSectionAssignments;

public record GetSectionAssignmentsQuery(
    Guid SectionId,
    bool? IsActive = null) : IRequest<ErrorOr<List<TeacherAssignmentResult>>>;
