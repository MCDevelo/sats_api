using ErrorOr;
using MediatR;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Students.Commands.CreateStudent;

namespace SchoolERP.Application.Students.Queries.GetStudents;

public record GetStudentsQuery : PagedQuery, IRequest<ErrorOr<PagedResult<StudentResult>>>
{
    public Guid? SchoolId { get; init; }
    public Guid? SectionId { get; init; }
    public Guid? GradeLevelId { get; init; }
    public bool? IsActive { get; init; } = true;
}
