using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Models;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Reports.Queries.GetEnrollmentReport;

public class GetEnrollmentReportQueryHandler
    : IRequestHandler<GetEnrollmentReportQuery, ErrorOr<ReportResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IReportGeneratorService _reportGenerator;

    public GetEnrollmentReportQueryHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        IReportGeneratorService reportGenerator)
    {
        _db = db;
        _currentUser = currentUser;
        _reportGenerator = reportGenerator;
    }

    public async Task<ErrorOr<ReportResult>> Handle(
        GetEnrollmentReportQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var school = await _db.Schools
            .FirstOrDefaultAsync(s =>
                s.Id == request.SchoolId &&
                s.TenantId == tenantId, cancellationToken);

        if (school is null)
            return Error.NotFound(description: "Centro educativo no encontrado.");

        var academicYear = await _db.AcademicYears
            .FirstOrDefaultAsync(y =>
                y.Id == request.AcademicYearId &&
                y.TenantId == tenantId, cancellationToken);

        if (academicYear is null)
            return Error.NotFound(description: "Año académico no encontrado.");

        // Sections for this school+year with their grade level and enrollments
        var sections = await _db.Sections
            .Include(s => s.GradeLevel)
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Student)
            .Where(s =>
                s.SchoolId == request.SchoolId &&
                s.AcademicYearId == request.AcademicYearId &&
                s.TenantId == tenantId &&
                s.IsActive)
            .OrderBy(s => s.GradeLevel.Order)
                .ThenBy(s => s.Name)
            .ToListAsync(cancellationToken);

        // Group by grade level
        var gradeLevelRows = sections
            .GroupBy(s => s.GradeLevel, (gl, sectionList) =>
            {
                var sectionRows = sectionList.Select(sec =>
                {
                    var active = sec.Enrollments
                        .Where(e => e.Status == EnrollmentStatus.Active)
                        .ToList();
                    return new SectionEnrollmentRow(
                        SectionName: sec.Name,
                        Enrolled: active.Count,
                        Male: active.Count(e => e.Student.Gender == Gender.M),
                        Female: active.Count(e => e.Student.Gender == Gender.F));
                }).ToList();

                return new GradeLevelEnrollmentRow(
                    GradeName: gl.Name,
                    Sections: sectionRows,
                    TotalEnrolled: sectionRows.Sum(s => s.Enrolled),
                    TotalMale: sectionRows.Sum(s => s.Male),
                    TotalFemale: sectionRows.Sum(s => s.Female));
            }).ToList();

        var data = new EnrollmentReportData(
            SchoolName: school.Name,
            AcademicYearName: academicYear.Name,
            GeneratedAt: DateTime.UtcNow,
            TotalEnrolled: gradeLevelRows.Sum(g => g.TotalEnrolled),
            GradeLevels: gradeLevelRows);

        var pdf = _reportGenerator.GenerateEnrollmentReport(data);
        var fileName = $"matricula_{academicYear.Name.Replace("-", "_")}_{DateTime.UtcNow:yyyyMMdd}.pdf";

        return new ReportResult(pdf, fileName);
    }
}
