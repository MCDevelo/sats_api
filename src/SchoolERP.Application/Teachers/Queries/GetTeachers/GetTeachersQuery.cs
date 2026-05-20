using ErrorOr;
using MediatR;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Teachers.Commands.CreateTeacher;

namespace SchoolERP.Application.Teachers.Queries.GetTeachers;

public record GetTeachersQuery : PagedQuery, IRequest<ErrorOr<PagedResult<TeacherResult>>>
{
    public Guid? SchoolId { get; init; }
    public bool? IsActive { get; init; } = true;
}
