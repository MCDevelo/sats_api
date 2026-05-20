using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Teachers.Queries.GetTeacherAssignments;

public class GetTeacherAssignmentsQueryHandler
    : IRequestHandler<GetTeacherAssignmentsQuery, ErrorOr<List<TeacherAssignmentResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetTeacherAssignmentsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<List<TeacherAssignmentResult>>> Handle(
        GetTeacherAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var teacherExists = await _db.Teachers
            .AnyAsync(t => t.Id == request.TeacherId && t.TenantId == tenantId, cancellationToken);

        if (!teacherExists)
            return Error.NotFound("Teacher.NotFound", "Docente no encontrado.");

        var query = _db.TeacherAssignments
            .Where(a => a.TeacherId == request.TeacherId);

        if (request.AcademicYearId.HasValue)
            query = query.Where(a => a.AcademicYearId == request.AcademicYearId.Value);

        if (request.IsActive.HasValue)
            query = query.Where(a => a.IsActive == request.IsActive.Value);
        else
            query = query.Where(a => a.IsActive);

        var items = await query
            .OrderByDescending(a => a.AcademicYear.StartDate)
            .ThenBy(a => a.Section.GradeLevel.Order)
            .ThenBy(a => a.Section.Name)
            .ThenBy(a => a.Subject.Name)
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
