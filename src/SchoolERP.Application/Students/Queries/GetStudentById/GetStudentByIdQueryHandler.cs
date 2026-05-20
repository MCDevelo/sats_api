using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Students.Queries.GetStudentById;

public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, ErrorOr<StudentDetailResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetStudentByIdQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<StudentDetailResult>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var student = await _db.Students
            .AsNoTracking()
            .Include(s => s.School)
            .Include(s => s.StudentGuardians)
                .ThenInclude(sg => sg.Guardian)
            .Include(s => s.Enrollments.Where(e => e.Status == EnrollmentStatus.Active))
                .ThenInclude(e => e.Section)
                    .ThenInclude(sec => sec.GradeLevel)
            .Include(s => s.Enrollments.Where(e => e.Status == EnrollmentStatus.Active))
                .ThenInclude(e => e.Section)
                    .ThenInclude(sec => sec.HomeTeacher)
            .Include(s => s.Enrollments.Where(e => e.Status == EnrollmentStatus.Active))
                .ThenInclude(e => e.AcademicYear)
            .FirstOrDefaultAsync(s => s.Id == request.StudentId && s.TenantId == tenantId, cancellationToken);

        if (student is null)
            return Error.NotFound(description: "Estudiante no encontrado.");

        var guardians = student.StudentGuardians
            .OrderByDescending(sg => sg.IsPrimary)
            .ThenBy(sg => sg.Guardian.LastName)
            .Select(sg => new GuardianSummary(
                GuardianId: sg.GuardianId,
                FullName: sg.Guardian.FullName,
                Relationship: sg.Relationship,
                Phone: sg.Guardian.Phone,
                Email: sg.Guardian.Email,
                IsPrimary: sg.IsPrimary,
                CanPickup: sg.CanPickup,
                IsFinancialResponsible: sg.Guardian.IsFinancialResponsible))
            .ToList();

        var activeEnrollment = student.Enrollments.FirstOrDefault();
        EnrollmentSummary? enrollmentSummary = null;

        if (activeEnrollment is not null)
        {
            enrollmentSummary = new EnrollmentSummary(
                EnrollmentId: activeEnrollment.Id,
                AcademicYear: activeEnrollment.AcademicYear.Name,
                GradeLevel: activeEnrollment.Section.GradeLevel.Name,
                Section: activeEnrollment.Section.Name,
                Shift: activeEnrollment.Section.Shift.ToString(),
                Status: activeEnrollment.Status.ToString(),
                HomeroomTeacher: activeEnrollment.Section.HomeTeacher?.FullName);
        }

        return new StudentDetailResult(
            Id: student.Id,
            TenantId: student.TenantId,
            SchoolId: student.SchoolId,
            SchoolName: student.School.Name,
            FirstName: student.FirstName,
            LastName: student.LastName,
            SecondLastName: student.SecondLastName,
            FullName: student.FullName,
            DateOfBirth: student.DateOfBirth,
            Age: student.Age,
            Gender: student.Gender.ToString(),
            NationalId: student.NationalId,
            Nse: student.Nse,
            StudentCode: student.StudentCode,
            Address: student.Address,
            Province: student.Province,
            Municipality: student.Municipality,
            Phone: student.Phone,
            BloodType: student.BloodType,
            Allergies: student.Allergies,
            MedicalNotes: student.MedicalNotes,
            HealthInsurance: student.HealthInsurance,
            HealthInsuranceNumber: student.HealthInsuranceNumber,
            HasSpecialNeeds: student.HasSpecialNeeds,
            Nationality: student.Nationality,
            PhotoUrl: student.PhotoUrl,
            IsActive: student.IsActive,
            CreatedAt: student.CreatedAt,
            Guardians: guardians,
            CurrentEnrollment: enrollmentSummary);
    }
}
