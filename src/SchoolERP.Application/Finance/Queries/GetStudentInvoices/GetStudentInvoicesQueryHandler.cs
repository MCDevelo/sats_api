using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Finance.Commands.CreateInvoice;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Finance.Queries.GetStudentInvoices;

public class GetStudentInvoicesQueryHandler : IRequestHandler<GetStudentInvoicesQuery, ErrorOr<PagedResult<InvoiceResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetStudentInvoicesQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<PagedResult<InvoiceResult>>> Handle(GetStudentInvoicesQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var studentExists = await _db.Students
            .AnyAsync(s => s.Id == request.StudentId && s.TenantId == tenantId, cancellationToken);

        if (!studentExists)
            return Error.NotFound(description: "Estudiante no encontrado.");

        var query = _db.Invoices
            .AsNoTracking()
            .Include(i => i.Student)
            .Where(i => i.StudentId == request.StudentId && i.TenantId == tenantId);

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<InvoiceStatus>(request.Status, true, out var status))
            query = query.Where(i => i.Status == status);

        if (request.Year.HasValue)
            query = query.Where(i => i.Year == request.Year.Value);

        var total = await query.CountAsync(cancellationToken);

        var invoices = await query
            .OrderByDescending(i => i.Year)
            .ThenByDescending(i => i.Month)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var items = invoices.Select(i => i.ToResult(i.Student.FullName)).ToList();

        return new PagedResult<InvoiceResult>(items, total, request.Page, request.PageSize);
    }
}
