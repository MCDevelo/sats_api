using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Enrollments.Queries.GetSectionEnrollments;

namespace SchoolERP.Application.Enrollments.Queries.GetStudentEnrollments;

public class GetStudentEnrollmentsQueryHandler
    : IRequestHandler<GetStudentEnrollmentsQuery, ErrorOr<List<EnrollmentResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetStudentEnrollmentsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<List<EnrollmentResult>>> Handle(
        GetStudentEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var studentExists = await _db.Students
            .AnyAsync(s => s.Id == request.StudentId && s.TenantId == tenantId, cancellationToken);

        if (!studentExists)
            return Error.NotFound("Student.NotFound", "Estudiante no encontrado.");

        var rows = await _db.Enrollments
            .Where(e => e.StudentId == request.StudentId && e.TenantId == tenantId)
            .OrderByDescending(e => e.AcademicYear.StartDate)
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

        return items;
    }
}
