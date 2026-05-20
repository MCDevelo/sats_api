using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Models;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Finance.Queries.GetOverdueInvoices;

public class GetOverdueInvoicesQueryHandler : IRequestHandler<GetOverdueInvoicesQuery, ErrorOr<PagedResult<OverdueInvoiceRow>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetOverdueInvoicesQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<PagedResult<OverdueInvoiceRow>>> Handle(GetOverdueInvoicesQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;
        var today = DateTime.UtcNow.Date;

        var query = _db.Invoices
            .AsNoTracking()
            .Include(i => i.Student)
                .ThenInclude(s => s.School)
            .Include(i => i.Student)
                .ThenInclude(s => s.StudentGuardians.Where(sg => sg.IsPrimary || sg.ReceivesNotifications))
                    .ThenInclude(sg => sg.Guardian)
            .Where(i =>
                i.TenantId == tenantId &&
                i.Status != InvoiceStatus.Paid &&
                i.Status != InvoiceStatus.Cancelled &&
                i.DueDate < today);

        if (request.SchoolId.HasValue)
            query = query.Where(i => i.Student.SchoolId == request.SchoolId.Value);

        if (request.AcademicYearId.HasValue)
            query = query.Where(i => i.AcademicYearId == request.AcademicYearId.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(i =>
                i.Student.FirstName.ToLower().Contains(search) ||
                i.Student.LastName.ToLower().Contains(search) ||
                i.InvoiceNumber.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var invoices = await query
            .OrderBy(i => i.DueDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var rows = invoices
            .Select(i =>
            {
                var daysOverdue = (today - i.DueDate.Date).Days;

                if (request.DaysOverdueMin.HasValue && daysOverdue < request.DaysOverdueMin.Value)
                    return null;

                var primaryGuardian = i.Student.StudentGuardians
                    .OrderByDescending(sg => sg.IsPrimary)
                    .FirstOrDefault();

                return new OverdueInvoiceRow(
                    InvoiceId: i.Id,
                    InvoiceNumber: i.InvoiceNumber,
                    StudentId: i.Student.Id,
                    StudentFullName: i.Student.FullName,
                    StudentCode: i.Student.StudentCode,
                    SchoolName: i.Student.School.Name,
                    Balance: i.Balance,
                    DueDate: i.DueDate,
                    DaysOverdue: daysOverdue,
                    GuardianPhone: primaryGuardian?.Guardian.Phone ?? string.Empty,
                    GuardianEmail: primaryGuardian?.Guardian.Email ?? string.Empty);
            })
            .Where(r => r is not null)
            .Cast<OverdueInvoiceRow>()
            .ToList();

        return new PagedResult<OverdueInvoiceRow>(rows, totalCount, request.Page, request.PageSize);
    }
}
