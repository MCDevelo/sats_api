using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Models;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Sections.Queries.GetSections;

public class GetSectionsQueryHandler : IRequestHandler<GetSectionsQuery, ErrorOr<PagedResult<SectionResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetSectionsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<PagedResult<SectionResult>>> Handle(GetSectionsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var query = _db.Sections.Where(s => s.TenantId == tenantId);

        if (request.SchoolId.HasValue)
            query = query.Where(s => s.SchoolId == request.SchoolId.Value);

        if (request.GradeLevelId.HasValue)
            query = query.Where(s => s.GradeLevelId == request.GradeLevelId.Value);

        if (request.AcademicYearId.HasValue)
            query = query.Where(s => s.AcademicYearId == request.AcademicYearId.Value);

        if (request.Shift.HasValue)
            query = query.Where(s => s.Shift == request.Shift.Value);

        if (request.IsActive.HasValue)
            query = query.Where(s => s.IsActive == request.IsActive.Value);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(s => s.GradeLevel.Order)
            .ThenBy(s => s.Name)
            .ThenBy(s => s.Shift)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new SectionResult(
                s.Id,
                s.SchoolId,
                s.GradeLevelId,
                s.GradeLevel.Name,
                s.AcademicYearId,
                s.AcademicYear.Name,
                s.Name,
                s.Shift.ToString(),
                s.Capacity,
                s.Enrollments.Count(e => e.Status == EnrollmentStatus.Active),
                s.HomeTeacherId,
                s.HomeTeacher != null ? s.HomeTeacher.FirstName + " " + s.HomeTeacher.LastName : null,
                s.Classroom,
                s.IsActive,
                s.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<SectionResult>(items, total, request.Page, request.PageSize);
    }
}
