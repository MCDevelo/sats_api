using ErrorOr;
using MediatR;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Finance.Commands.CreateInvoice;

namespace SchoolERP.Application.Finance.Queries.GetStudentInvoices;

public record GetStudentInvoicesQuery : PagedQuery, IRequest<ErrorOr<PagedResult<InvoiceResult>>>
{
    public Guid StudentId { get; init; }
    public string? Status { get; init; }
    public int? Year { get; init; }
}
