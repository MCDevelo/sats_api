using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid TenantId { get; private set; }
    public Guid InvoiceId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string? Reference { get; private set; } // Transaction ID, cheque number, etc.
    public DateTime PaidAt { get; private set; }
    public Guid? ReceivedBy { get; private set; } // UserId of cashier
    public string? Notes { get; private set; }

    // Navigation
    public Tenant Tenant { get; private set; } = default!;
    public Invoice Invoice { get; private set; } = default!;

    private Payment() { }

    public static Payment Create(
        Guid tenantId,
        Guid invoiceId,
        decimal amount,
        PaymentMethod method,
        DateTime? paidAt = null,
        string? reference = null,
        Guid? receivedBy = null,
        string? notes = null)
    {
        if (amount <= 0)
            throw new ArgumentException("El monto del pago debe ser mayor a 0.");

        return new Payment
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            InvoiceId = invoiceId,
            Amount = amount,
            Method = method,
            PaidAt = paidAt ?? DateTime.UtcNow,
            Reference = reference,
            ReceivedBy = receivedBy,
            Notes = notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
