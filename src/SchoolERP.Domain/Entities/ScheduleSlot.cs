using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

public class ScheduleSlot : BaseEntity
{
    public Guid TenantId { get; private set; }

    /// <summary>
    /// Links Teacher + Section + Subject + AcademicYear in one FK.
    /// </summary>
    public Guid TeacherAssignmentId { get; private set; }

    /// <summary>DayOfWeek: 1=Monday … 5=Friday (0=Sunday excluded for school use)</summary>
    public DayOfWeek Day { get; private set; }

    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }

    /// <summary>Optional room override (falls back to Section.Classroom).</summary>
    public string? Room { get; private set; }

    // Navigation
    public TeacherAssignment TeacherAssignment { get; private set; } = default!;

    private ScheduleSlot() { }

    public static ScheduleSlot Create(
        Guid tenantId,
        Guid teacherAssignmentId,
        DayOfWeek day,
        TimeOnly startTime,
        TimeOnly endTime,
        string? room = null)
    {
        if (endTime <= startTime)
            throw new ArgumentException("EndTime must be after StartTime.");

        return new ScheduleSlot
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            TeacherAssignmentId = teacherAssignmentId,
            Day = day,
            StartTime = startTime,
            EndTime = endTime,
            Room = room,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(DayOfWeek day, TimeOnly startTime, TimeOnly endTime, string? room)
    {
        if (endTime <= startTime)
            throw new ArgumentException("EndTime must be after StartTime.");

        Day = day;
        StartTime = startTime;
        EndTime = endTime;
        Room = room;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Returns true if this slot's time range overlaps with the given range on the same day.</summary>
    public bool OverlapsWith(DayOfWeek day, TimeOnly start, TimeOnly end)
        => Day == day && StartTime < end && EndTime > start;
}
