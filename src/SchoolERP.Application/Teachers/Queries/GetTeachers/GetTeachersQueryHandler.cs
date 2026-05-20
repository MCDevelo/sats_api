using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Teachers.Commands.CreateTeacher;

namespace SchoolERP.Application.Teachers.Queries.GetTeachers;

public class GetTeachersQueryHandler : IRequestHandler<GetTeachersQuery, ErrorOr<PagedResult<TeacherResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetTeachersQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<PagedResult<TeacherResult>>> Handle(GetTeachersQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var query = _db.Teachers
            .AsNoTracking()
            .Where(t => t.TenantId == tenantId);

        if (request.SchoolId.HasValue)
            query = query.Where(t => t.SchoolId == request.SchoolId.Value);

        if (request.IsActive.HasValue)
            query = query.Where(t => t.IsActive == request.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower().Trim();
            query = query.Where(t =>
                t.FirstName.ToLower().Contains(search) ||
                t.LastName.ToLower().Contains(search) ||
                (t.Email != null && t.Email.Contains(search)) ||
                (t.NationalId != null && t.NationalId.Contains(search)) ||
                (t.TeacherCode != null && t.TeacherCode.Contains(search)));
        }

        query = request.SortBy switch
        {
            "firstName" => request.SortDesc ? query.OrderByDescending(t => t.FirstName) : query.OrderBy(t => t.FirstName),
            "lastName" => request.SortDesc ? query.OrderByDescending(t => t.LastName) : query.OrderBy(t => t.LastName),
            "hireDate" => request.SortDesc ? query.OrderByDescending(t => t.HireDate) : query.OrderBy(t => t.HireDate),
            _ => query.OrderBy(t => t.LastName).ThenBy(t => t.FirstName)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var teachers = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var items = teachers.Select(t => t.ToResult()).ToList();

        return new PagedResult<TeacherResult>(items, totalCount, request.Page, request.PageSize);
    }
}
