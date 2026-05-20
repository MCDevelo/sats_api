using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Students.Commands.CreateStudent;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Students.Queries.GetStudents;

public class GetStudentsQueryHandler : IRequestHandler<GetStudentsQuery, ErrorOr<PagedResult<StudentResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetStudentsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<PagedResult<StudentResult>>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var query = _db.Students
            .AsNoTracking()
            .Where(s => s.TenantId == tenantId);

        if (request.SchoolId.HasValue)
            query = query.Where(s => s.SchoolId == request.SchoolId.Value);

        if (request.IsActive.HasValue)
            query = query.Where(s => s.IsActive == request.IsActive.Value);

        if (request.SectionId.HasValue)
            query = query.Where(s => s.Enrollments.Any(
                e => e.SectionId == request.SectionId.Value && e.Status == EnrollmentStatus.Active));

        if (request.GradeLevelId.HasValue)
            query = query.Where(s => s.Enrollments.Any(
                e => e.Section.GradeLevelId == request.GradeLevelId.Value && e.Status == EnrollmentStatus.Active));

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower().Trim();
            query = query.Where(s =>
                s.FirstName.ToLower().Contains(search) ||
                s.LastName.ToLower().Contains(search) ||
                (s.SecondLastName != null && s.SecondLastName.ToLower().Contains(search)) ||
                (s.NationalId != null && s.NationalId.Contains(search)) ||
                (s.Nse != null && s.Nse.Contains(search)) ||
                (s.StudentCode != null && s.StudentCode.Contains(search)));
        }

        query = request.SortBy switch
        {
            "firstName" => request.SortDesc ? query.OrderByDescending(s => s.FirstName) : query.OrderBy(s => s.FirstName),
            "lastName"  => request.SortDesc ? query.OrderByDescending(s => s.LastName)  : query.OrderBy(s => s.LastName),
            "dateOfBirth" => request.SortDesc ? query.OrderByDescending(s => s.DateOfBirth) : query.OrderBy(s => s.DateOfBirth),
            _ => query.OrderBy(s => s.LastName).ThenBy(s => s.FirstName)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        // Project to DTO including primary guardian — avoids loading full navigation graph
        var rows = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new
            {
                s.Id,
                s.TenantId,
                s.SchoolId,
                s.FirstName,
                s.LastName,
                s.SecondLastName,
                s.DateOfBirth,
                s.Gender,
                s.NationalId,
                s.Nse,
                s.StudentCode,
                s.Address,
                s.Province,
                s.Municipality,
                s.Phone,
                s.BloodType,
                s.Allergies,
                s.MedicalNotes,
                s.HealthInsurance,
                s.HealthInsuranceNumber,
                s.HasSpecialNeeds,
                s.Nationality,
                s.PhotoUrl,
                s.IsActive,
                s.CreatedAt,
                PrimaryFirstName = s.StudentGuardians
                    .Where(sg => sg.IsPrimary)
                    .Select(sg => sg.Guardian.FirstName)
                    .FirstOrDefault(),
                PrimaryLastName = s.StudentGuardians
                    .Where(sg => sg.IsPrimary)
                    .Select(sg => sg.Guardian.LastName)
                    .FirstOrDefault(),
                PrimaryPhone = s.StudentGuardians
                    .Where(sg => sg.IsPrimary)
                    .Select(sg => sg.Guardian.Phone)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        var items = rows.Select(r => new StudentResult(
            r.Id,
            r.TenantId,
            r.SchoolId,
            r.FirstName,
            r.LastName,
            r.SecondLastName,
            string.IsNullOrEmpty(r.SecondLastName)
                ? $"{r.FirstName} {r.LastName}"
                : $"{r.FirstName} {r.LastName} {r.SecondLastName}",
            r.DateOfBirth,
            (int)((DateTime.UtcNow - r.DateOfBirth).TotalDays / 365.25),
            r.Gender.ToString(),
            r.NationalId,
            r.Nse,
            r.StudentCode,
            r.Address,
            r.Province,
            r.Municipality,
            r.Phone,
            r.BloodType,
            r.Allergies,
            r.MedicalNotes,
            r.HealthInsurance,
            r.HealthInsuranceNumber,
            r.HasSpecialNeeds,
            r.Nationality,
            r.PhotoUrl,
            r.IsActive,
            r.CreatedAt,
            r.PrimaryFirstName != null
                ? new GuardianPrimary(
                    $"{r.PrimaryFirstName} {r.PrimaryLastName}",
                    r.PrimaryPhone)
                : null))
        .ToList();

        return new PagedResult<StudentResult>(items, totalCount, request.Page, request.PageSize);
    }
}
