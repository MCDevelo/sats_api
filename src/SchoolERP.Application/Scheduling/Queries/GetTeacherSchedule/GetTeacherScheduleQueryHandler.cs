using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Scheduling.Queries.GetTeacherSchedule;

public class GetTeacherScheduleQueryHandler
    : IRequestHandler<GetTeacherScheduleQuery, ErrorOr<TeacherScheduleResult>>
{
    private static readonly string[] DayNames =
        ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"];

    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetTeacherScheduleQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<TeacherScheduleResult>> Handle(
        GetTeacherScheduleQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var teacher = await _db.Teachers
            .FirstOrDefaultAsync(t =>
                t.Id == request.TeacherId &&
                t.TenantId == tenantId, cancellationToken);

        if (teacher is null)
            return Error.NotFound(description: "Docente no encontrado.");

        var slots = await _db.ScheduleSlots
            .Include(s => s.TeacherAssignment)
                .ThenInclude(a => a.Subject)
            .Include(s => s.TeacherAssignment)
                .ThenInclude(a => a.Section)
                    .ThenInclude(sec => sec.GradeLevel)
            .Where(s =>
                s.TenantId == tenantId &&
                s.TeacherAssignment.TeacherId == request.TeacherId &&
                s.TeacherAssignment.AcademicYearId == request.AcademicYearId &&
                s.TeacherAssignment.IsActive)
            .OrderBy(s => s.Day)
                .ThenBy(s => s.StartTime)
            .ToListAsync(cancellationToken);

        var slotResults = slots.Select(s => new TeacherSlotResult(
            SlotId: s.Id,
            Day: s.Day,
            DayName: DayNames[(int)s.Day],
            StartTime: s.StartTime,
            EndTime: s.EndTime,
            SubjectName: s.TeacherAssignment.Subject.Name,
            SectionName: s.TeacherAssignment.Section.Name,
            GradeLevel: s.TeacherAssignment.Section.GradeLevel.Name,
            Room: s.Room ?? s.TeacherAssignment.Section.Classroom))
            .ToList();

        return new TeacherScheduleResult(
            TeacherId: teacher.Id,
            TeacherFullName: teacher.FullName,
            Slots: slotResults);
    }
}
