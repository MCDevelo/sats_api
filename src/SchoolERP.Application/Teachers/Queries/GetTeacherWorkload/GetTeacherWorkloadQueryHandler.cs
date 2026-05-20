using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Teachers.Queries.GetTeacherWorkload;

public class GetTeacherWorkloadQueryHandler
    : IRequestHandler<GetTeacherWorkloadQuery, ErrorOr<TeacherWorkloadResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetTeacherWorkloadQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<TeacherWorkloadResult>> Handle(
        GetTeacherWorkloadQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var teacher = await _db.Teachers
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.TeacherId && t.TenantId == tenantId, cancellationToken);

        if (teacher is null)
            return Error.NotFound("Teacher.NotFound", "Docente no encontrado.");

        // Resolve academic year: use requested, or the latest active one
        Guid academicYearId;
        string academicYearName;

        if (request.AcademicYearId.HasValue)
        {
            var ay = await _db.AcademicYears
                .AsNoTracking()
                .FirstOrDefaultAsync(y => y.Id == request.AcademicYearId.Value && y.TenantId == tenantId, cancellationToken);

            if (ay is null)
                return Error.NotFound("AcademicYear.NotFound", "Año académico no encontrado.");

            academicYearId = ay.Id;
            academicYearName = ay.Name;
        }
        else
        {
            var current = await _db.AcademicYears
                .AsNoTracking()
                .Where(y => y.TenantId == tenantId && y.IsActive)
                .OrderByDescending(y => y.StartDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (current is null)
                return Error.NotFound("AcademicYear.NotFound", "No hay año académico activo.");

            academicYearId = current.Id;
            academicYearName = current.Name;
        }

        // Load active assignments for this teacher + academic year with schedule slots
        var assignments = await _db.TeacherAssignments
            .AsNoTracking()
            .Where(a => a.TeacherId == request.TeacherId
                     && a.AcademicYearId == academicYearId
                     && a.IsActive)
            .Include(a => a.Section)
                .ThenInclude(s => s.GradeLevel)
            .Include(a => a.Subject)
            .ToListAsync(cancellationToken);

        if (assignments.Count == 0)
        {
            return new TeacherWorkloadResult(
                TeacherId: teacher.Id,
                TeacherName: teacher.FullName,
                NationalId: teacher.NationalId,
                AcademicYear: academicYearName,
                TotalWeeklyHours: 0,
                MaxWeeklyHours: teacher.WorkingHoursPerWeek,
                IsOverloaded: false,
                Sections: []);
        }

        var assignmentIds = assignments.Select(a => a.Id).ToList();

        // Load schedule slots for these assignments
        var slots = await _db.ScheduleSlots
            .AsNoTracking()
            .Where(ss => assignmentIds.Contains(ss.TeacherAssignmentId))
            .ToListAsync(cancellationToken);

        // Group slots by assignment and compute weekly hours
        var slotsByAssignment = slots
            .GroupBy(ss => ss.TeacherAssignmentId)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(ss => (ss.EndTime - ss.StartTime).TotalHours));

        // Get student counts per section
        var sectionIds = assignments.Select(a => a.SectionId).Distinct().ToList();
        var studentCounts = await _db.Enrollments
            .Where(e => sectionIds.Contains(e.SectionId) && e.Status == EnrollmentStatus.Active)
            .GroupBy(e => e.SectionId)
            .Select(g => new { SectionId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var countBySectionId = studentCounts.ToDictionary(x => x.SectionId, x => x.Count);

        var sections = assignments
            .OrderBy(a => a.Section.GradeLevel.Order)
            .ThenBy(a => a.Section.Name)
            .ThenBy(a => a.Subject.Name)
            .Select(a => new WorkloadSectionItem(
                AssignmentId: a.Id,
                GradeLevel: a.Section.GradeLevel.Name,
                Section: a.Section.Name,
                Subject: a.Subject.Name,
                Shift: a.Section.Shift.ToString(),
                WeeklyHours: slotsByAssignment.TryGetValue(a.Id, out var h) ? Math.Round(h, 1) : 0,
                Students: countBySectionId.TryGetValue(a.SectionId, out var cnt) ? cnt : 0))
            .ToList();

        var totalWeeklyHours = Math.Round(sections.Sum(s => s.WeeklyHours), 1);

        return new TeacherWorkloadResult(
            TeacherId: teacher.Id,
            TeacherName: teacher.FullName,
            NationalId: teacher.NationalId,
            AcademicYear: academicYearName,
            TotalWeeklyHours: totalWeeklyHours,
            MaxWeeklyHours: teacher.WorkingHoursPerWeek,
            IsOverloaded: totalWeeklyHours > teacher.WorkingHoursPerWeek,
            Sections: sections);
    }
}
