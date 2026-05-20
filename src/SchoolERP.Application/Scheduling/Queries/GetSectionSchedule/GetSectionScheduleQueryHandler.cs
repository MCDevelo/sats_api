using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Scheduling.Queries.GetSectionSchedule;

public class GetSectionScheduleQueryHandler
    : IRequestHandler<GetSectionScheduleQuery, ErrorOr<SectionScheduleResult>>
{
    private static readonly string[] DayNames =
        ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"];

    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetSectionScheduleQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<SectionScheduleResult>> Handle(
        GetSectionScheduleQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var section = await _db.Sections
            .Include(s => s.GradeLevel)
            .FirstOrDefaultAsync(s =>
                s.Id == request.SectionId &&
                s.TenantId == tenantId, cancellationToken);

        if (section is null)
            return Error.NotFound(description: "Sección no encontrada.");

        var slots = await _db.ScheduleSlots
            .Include(s => s.TeacherAssignment)
                .ThenInclude(a => a.Teacher)
            .Include(s => s.TeacherAssignment)
                .ThenInclude(a => a.Subject)
            .Where(s =>
                s.TenantId == tenantId &&
                s.TeacherAssignment.SectionId == request.SectionId &&
                s.TeacherAssignment.AcademicYearId == request.AcademicYearId &&
                s.TeacherAssignment.IsActive)
            .OrderBy(s => s.Day)
                .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);

        var slotResults = slots.Select(s => new ScheduleSlotResult(
            SlotId: s.Id,
            Day: s.Day,
            DayName: DayNames[(int)s.Day],
            StartTime: s.StartTime,
            EndTime: s.EndTime,
            SubjectName: s.TeacherAssignment.Subject.Name,
            TeacherFullName: s.TeacherAssignment.Teacher.FullName,
            Room: s.Room ?? section.Classroom))
            .ToList();

        return new SectionScheduleResult(
            SectionId: section.Id,
            SectionName: section.Name,
            GradeLevel: section.GradeLevel.Name,
            Slots: slotResults);
    }
}
