using SchoolERP.Application.Finance.Commands.CreateInvoice;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Finance;

public static class FinanceMappings
{
    public static InvoiceResult ToResult(this Invoice i, string studentFullName) => new(
        Id: i.Id,
        StudentId: i.StudentId,
        StudentFullName: studentFullName,
        InvoiceNumber: i.InvoiceNumber,
        Ncf: i.Ncf,
        Description: i.Description,
        Amount: i.Amount,
        Discount: i.Discount,
        Balance: i.Balance,
        AmountPaid: i.AmountPaid,
        Status: i.Status.ToString(),
        DueDate: i.DueDate,
        Month: i.Month,
        Year: i.Year,
        CreatedAt: i.CreatedAt);
}
