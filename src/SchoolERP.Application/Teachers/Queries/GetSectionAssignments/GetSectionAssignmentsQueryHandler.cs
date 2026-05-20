using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Teachers.Queries.GetTeacherAssignments;

namespace SchoolERP.Application.Teachers.Queries.GetSectionAssignments;

public class GetSectionAssignmentsQueryHandler
    : IRequestHandler<GetSectionAssignmentsQuery, ErrorOr<List<TeacherAssignmentResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetSectionAssignmentsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<List<TeacherAssignmentResult>>> Handle(
        GetSectionAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var sectionExists = await _db.Sections
            .AnyAsync(s => s.Id == request.SectionId && s.TenantId == tenantId, cancellationToken);

        if (!sectionExists)
            return Error.NotFound("Section.NotFound", "Sección no encontrada.");

        var query = _db.TeacherAssignments
            .Where(a => a.SectionId == request.SectionId);

        if (request.IsActive.HasValue)
            query = query.Where(a => a.IsActive == request.IsActive.Value);
        else
            query = query.Where(a => a.IsActive);

        var items = await query
            .OrderBy(a => a.Subject.Name)
            .Select(a => new TeacherAssignmentResult(
                a.Id,
                a.TeacherId,
                a.Teacher.FirstName + " " + a.Teacher.LastName,
                a.SectionId,
                a.Section.Name,
                a.Section.Shift,
                a.Section.GradeLevelId,
                a.Section.GradeLevel.Name,
                a.SubjectId,
                a.Subject.Name,
                a.AcademicYearId,
                a.AcademicYear.Name,
                a.IsActive,
                a.CreatedAt))
            .ToListAsync(cancellationToken);

        return items;
    }
}
