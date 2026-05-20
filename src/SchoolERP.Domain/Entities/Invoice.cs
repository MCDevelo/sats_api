using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

public class Invoice : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid AcademicYearId { get; private set; }
    public string InvoiceNumber { get; private set; } = default!;
    public string? Ncf { get; private set; } // Número de Comprobante Fiscal (DGII)
    public string Description { get; private set; } = default!;
    public decimal Amount { get; private set; }
    public decimal AmountPaid { get; private set; }
    public decimal Discount { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public string? Notes { get; private set; }
    public int Month { get; private set; } // 1-12
    public int Year { get; private set; }

    // Navigation
    public Tenant Tenant { get; private set; } = default!;
    public Student Student { get; private set; } = default!;
    public AcademicYear AcademicYear { get; private set; } = default!;
    public ICollection<Payment> Payments { get; private set; } = [];

    private Invoice() { }

    public static Invoice Create(
        Guid tenantId,
        Guid studentId,
        Guid academicYearId,
        string invoiceNumber,
        string description,
        decimal amount,
        DateTime dueDate,
        int month,
        int year,
        decimal discount = 0,
        string? ncf = null)
    {
        return new Invoice
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            StudentId = studentId,
            AcademicYearId = academicYearId,
            InvoiceNumber = invoiceNumber,
            Description = description,
            Amount = amount,
            DueDate = dueDate,
            Month = month,
            Year = year,
            Discount = discount,
            Ncf = ncf,
            Status = InvoiceStatus.Pending,
            AmountPaid = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public decimal Balance => Amount - Discount - AmountPaid;
    public bool IsOverdue => Status == InvoiceStatus.Pending && DateTime.UtcNow > DueDate;

    public void ApplyPayment(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("El monto del pago debe ser mayor a 0.");
        if (amount > Balance) throw new InvalidOperationException("El pago excede el balance pendiente.");

        AmountPaid += amount;

        if (AmountPaid >= Amount - Discount)
        {
            Status = InvoiceStatus.Paid;
            PaidAt = DateTime.UtcNow;
        }
        else
        {
            Status = InvoiceStatus.Partial;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkOverdue()
    {
        if (Status == InvoiceStatus.Pending)
        {
            Status = InvoiceStatus.Overdue;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void Cancel(string? reason = null)
    {
        Status = InvoiceStatus.Cancelled;
        Notes = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ApplyDiscount(decimal discount)
    {
        Discount = discount;
        UpdatedAt = DateTime.UtcNow;
    }
}
