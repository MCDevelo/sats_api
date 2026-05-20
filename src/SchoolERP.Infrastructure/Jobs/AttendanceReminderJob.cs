using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Jobs;

/// <summary>
/// Sends InApp notifications to home teachers of sections that have not
/// recorded attendance for the current school day.
/// Scheduled: Mon–Fri at 18:00 UTC (≈ 2:00 PM Dominican Republic, UTC-4).
/// </summary>
public class AttendanceReminderJob
{
    private readonly IApplicationDbContext _db;
    private readonly ILogger<AttendanceReminderJob> _logger;

    public AttendanceReminderJob(IApplicationDbContext db, ILogger<AttendanceReminderJob> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        // Dominican Republic is UTC-4 (no daylight saving)
        var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(-4));

        // Active sections in the current academic year
        var sections = await _db.Sections
            .Include(s => s.AcademicYear)
            .Where(s =>
                s.IsActive &&
                s.AcademicYear.IsCurrent &&
                s.HomeTeacherId.HasValue)
            .ToListAsync();

        // Sections that already have at least one attendance record today
        var sectionsWithAttendance = await _db.AttendanceRecords
            .Where(r => r.Date == today)
            .Select(r => r.SectionId)
            .Distinct()
            .ToListAsync();

        var missingSections = sections
            .Where(s => !sectionsWithAttendance.Contains(s.Id))
            .ToList();

        if (missingSections.Count == 0)
        {
            _logger.LogInformation(
                "AttendanceReminderJob: all sections have recorded attendance for {Date}.", today);
            return;
        }

        var notifications = missingSections.Select(s => Notification.Create(
            tenantId: s.TenantId,
            channel: "InApp",
            eventType: "AttendanceReminder",
            title: "Asistencia pendiente",
            body: $"No ha registrado la asistencia del día de hoy ({today:dd/MM/yyyy}) " +
                  $"para la sección {s.Name}. Por favor registre la asistencia antes del cierre del día.",
            recipientUserId: s.HomeTeacherId!.Value)).ToList();

        _db.Notifications.AddRange(notifications);
        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "AttendanceReminderJob: sent {Count} attendance reminder(s) for {Date}.",
            notifications.Count, today);
    }
}
