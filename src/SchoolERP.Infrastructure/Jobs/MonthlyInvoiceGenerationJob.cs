using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Infrastructure.Jobs;

/// <summary>
/// Generates monthly invoices for all active students enrolled in a school.
/// Skips students who already have an invoice for the given month/year.
/// Triggered manually via the Jobs API (not a recurring job).
/// </summary>
public class MonthlyInvoiceGenerationJob
{
    private readonly IApplicationDbContext _db;
    private readonly ILogger<MonthlyInvoiceGenerationJob> _logger;

    public MonthlyInvoiceGenerationJob(
        IApplicationDbContext db,
        ILogger<MonthlyInvoiceGenerationJob> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task ExecuteAsync(
        Guid schoolId,
        Guid academicYearId,
        int month,
        int year,
        decimal amount,
        string description)
    {
        _logger.LogInformation(
            "MonthlyInvoiceGenerationJob: starting for School={SchoolId} Period={Month}/{Year}",
            schoolId, month, year);

        // Validate school exists
        var school = await _db.Schools
            .FirstOrDefaultAsync(s => s.Id == schoolId);

        if (school is null)
        {
            _logger.LogWarning("MonthlyInvoiceGenerationJob: school {SchoolId} not found.", schoolId);
            return;
        }

        // Active enrollments for this school / academic year
        var enrollments = await _db.Enrollments
            .Include(e => e.Student)
            .Where(e =>
                e.Student.SchoolId == schoolId &&
                e.Section.AcademicYearId == academicYearId &&
                e.Status == EnrollmentStatus.Active)
            .ToListAsync();

        // Find already-invoiced students for this month/year
        var alreadyInvoicedStudentIds = await _db.Invoices
            .Where(i =>
                i.TenantId == school.TenantId &&
                i.Month == month &&
                i.Year == year &&
                i.Status != InvoiceStatus.Cancelled &&
                enrollments.Select(e => e.StudentId).Contains(i.StudentId))
            .Select(i => i.StudentId)
            .ToListAsync();

        var alreadyInvoicedSet = alreadyInvoicedStudentIds.ToHashSet();

        var toCreate = enrollments
            .Where(e => !alreadyInvoicedSet.Contains(e.StudentId))
            .ToList();

        if (toCreate.Count == 0)
        {
            _logger.LogInformation(
                "MonthlyInvoiceGenerationJob: all {Total} students already invoiced. Nothing to create.",
                enrollments.Count);
            return;
        }

        // Sequential invoice numbering within this batch
        var lastInvoiceNumber = await _db.Invoices
            .Where(i => i.TenantId == school.TenantId && i.Year == year)
            .OrderByDescending(i => i.InvoiceNumber)
            .Select(i => i.InvoiceNumber)
            .FirstOrDefaultAsync();

        var seq = ParseSequence(lastInvoiceNumber, year) + 1;

        var dueDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        var invoices = new List<Invoice>();

        foreach (var enrollment in toCreate)
        {
            var invoiceNumber = $"INV-{year}-{seq:D6}";
            seq++;

            invoices.Add(Invoice.Create(
                tenantId: school.TenantId,
                studentId: enrollment.StudentId,
                academicYearId: academicYearId,
                invoiceNumber: invoiceNumber,
                description: description,
                amount: amount,
                dueDate: dueDate,
                month: month,
                year: year));
        }

        _db.Invoices.AddRange(invoices);
        await _db.SaveChangesAsync();

        _logger.LogInformation(
            "MonthlyInvoiceGenerationJob: created {Created} invoices, skipped {Skipped} (already invoiced).",
            invoices.Count, alreadyInvoicedSet.Count);
    }

    private static int ParseSequence(string? lastInvoiceNumber, int year)
    {
        if (lastInvoiceNumber is null) return 0;
        // Format: INV-{year}-{seq:D6}
        var parts = lastInvoiceNumber.Split('-');
        if (parts.Length == 3 && parts[1] == year.ToString()
            && int.TryParse(parts[2], out var seq))
            return seq;
        return 0;
    }
}
