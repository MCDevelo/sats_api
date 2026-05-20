using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Infrastructure.Jobs;

/// <summary>
/// Marks Pending invoices whose DueDate has passed as Overdue.
/// Scheduled: daily at midnight UTC.
/// </summary>
public class OverdueInvoiceMarkingJob
{
    private readonly IApplicationDbContext _db;
    private readonly ILogger<OverdueInvoiceMarkingJob> _logger;

    public OverdueInvoiceMarkingJob(IApplicationDbContext db, ILogger<OverdueInvoiceMarkingJob> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var now = DateTime.UtcNow;

        var overdueInvoices = await _db.Invoices
            .Where(i => i.Status == InvoiceStatus.Pending && i.DueDate < now)
            .ToListAsync();

        if (overdueInvoices.Count == 0)
        {
            _logger.LogInformation("OverdueInvoiceMarkingJob: no pending overdue invoices found.");
            return;
        }

        foreach (var invoice in overdueInvoices)
            invoice.MarkOverdue();

        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "OverdueInvoiceMarkingJob: marked {Count} invoice(s) as overdue.",
            overdueInvoices.Count);
    }
}
