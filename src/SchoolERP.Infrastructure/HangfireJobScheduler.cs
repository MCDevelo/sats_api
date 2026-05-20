using Hangfire;
using SchoolERP.Infrastructure.Jobs;

namespace SchoolERP.Infrastructure;

/// <summary>
/// Registers all recurring Hangfire jobs at application startup.
/// Call RegisterRecurringJobs() once in Program.cs after the app is built.
/// </summary>
public class HangfireJobScheduler
{
    private readonly IRecurringJobManager _jobManager;

    public HangfireJobScheduler(IRecurringJobManager jobManager)
    {
        _jobManager = jobManager;
    }

    public void RegisterRecurringJobs()
    {
        // ── Daily at 00:05 UTC ────────────────────────────────────────────────
        _jobManager.AddOrUpdate<OverdueInvoiceMarkingJob>(
            recurringJobId: "overdue-invoice-marking",
            methodCall: job => job.ExecuteAsync(),
            cronExpression: "5 0 * * *",
            options: new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

        // ── Mon–Fri at 18:00 UTC (2:00 PM RD, UTC-4) ─────────────────────────
        _jobManager.AddOrUpdate<AttendanceReminderJob>(
            recurringJobId: "attendance-reminder",
            methodCall: job => job.ExecuteAsync(),
            cronExpression: "0 18 * * 1-5",
            options: new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

        // ── Every 30 minutes ─────────────────────────────────────────────────
        _jobManager.AddOrUpdate<NotificationRetryJob>(
            recurringJobId: "notification-retry",
            methodCall: job => job.ExecuteAsync(),
            cronExpression: "*/30 * * * *",
            options: new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });
    }
}
