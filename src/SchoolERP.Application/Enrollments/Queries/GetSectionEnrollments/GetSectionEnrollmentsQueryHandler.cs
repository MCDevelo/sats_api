using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Models;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Enrollments.Queries.GetSectionEnrollments;

public class GetSectionEnrollmentsQueryHandler
    : IRequestHandler<GetSectionEnrollmentsQuery, ErrorOr<PagedResult<EnrollmentResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetSectionEnrollmentsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<PagedResult<EnrollmentResult>>> Handle(
        GetSectionEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var sectionExists = await _db.Sections
            .AnyAsync(s => s.Id == request.SectionId && s.TenantId == tenantId, cancellationToken);

        if (!sectionExists)
            return Error.NotFound("Section.NotFound", "Sección no encontrada.");

        var query = _db.Enrollments
            .Where(e => e.SectionId == request.SectionId && e.TenantId == tenantId);

        if (request.Status.HasValue)
            query = query.Where(e => e.Status == request.Status.Value);
        else
            query = query.Where(e => e.Status == EnrollmentStatus.Active);

        var total = await query.CountAsync(cancellationToken);

        var rows = await query
            .OrderBy(e => e.Student.LastName)
            .ThenBy(e => e.Student.FirstName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new
            {
                e.Id,
                e.StudentId,
                e.Student.FirstName,
                e.Student.LastName,
                e.Student.SecondLastName,
                e.Student.StudentCode,
                e.Student.Nse,
                Gender = e.Student.Gender.ToString(),
                e.Student.PhotoUrl,
                e.SectionId,
                SectionName = e.Section.Name,
                e.AcademicYearId,
                AcademicYearName = e.AcademicYear.Name,
                e.Status,
                e.EnrollmentDate,
                e.WithdrawalDate,
                e.WithdrawalReason,
                e.Notes,
                e.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var items = rows.Select(e => new EnrollmentResult(
            e.Id,
            e.StudentId,
            string.IsNullOrEmpty(e.SecondLastName)
                ? e.FirstName + " " + e.LastName
                : e.FirstName + " " + e.LastName + " " + e.SecondLastName,
            e.StudentCode,
            e.Nse,
            e.Gender,
            e.PhotoUrl,
            e.SectionId,
            e.SectionName,
            e.AcademicYearId,
            e.AcademicYearName,
            e.Status,
            e.EnrollmentDate,
            e.WithdrawalDate,
            e.WithdrawalReason,
            e.Notes,
            e.CreatedAt))
            .ToList();

        return new PagedResult<EnrollmentResult>(items, total, request.Page, request.PageSize);
    }
}
