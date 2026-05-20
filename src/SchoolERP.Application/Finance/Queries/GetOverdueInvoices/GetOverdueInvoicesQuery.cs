using ErrorOr;
using MediatR;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Finance.Commands.CreateInvoice;

namespace SchoolERP.Application.Finance.Queries.GetOverdueInvoices;

public record GetOverdueInvoicesQuery : PagedQuery, IRequest<ErrorOr<PagedResult<OverdueInvoiceRow>>>
{
    public Guid? SchoolId { get; init; }
    public Guid? AcademicYearId { get; init; }
    public int? DaysOverdueMin { get; init; }
}

public record OverdueInvoiceRow(
    Guid InvoiceId,
    string InvoiceNumber,
    Guid StudentId,
    string StudentFullName,
    string? StudentCode,
    string SchoolName,
    decimal Balance,
    DateTime DueDate,
    int DaysOverdue,
    string GuardianPhone,
    string GuardianEmail);
