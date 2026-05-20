using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Teachers.Queries.GetTeacherById;

public class GetTeacherByIdQueryHandler : IRequestHandler<GetTeacherByIdQuery, ErrorOr<TeacherDetailResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetTeacherByIdQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<TeacherDetailResult>> Handle(GetTeacherByIdQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var teacher = await _db.Teachers
            .AsNoTracking()
            .Include(t => t.School)
            .Include(t => t.Assignments.Where(a => a.IsActive))
                .ThenInclude(a => a.Section)
                    .ThenInclude(s => s.GradeLevel)
            .Include(t => t.Assignments.Where(a => a.IsActive))
                .ThenInclude(a => a.Section)
                    .ThenInclude(s => s.AcademicYear)
            .Include(t => t.Assignments.Where(a => a.IsActive))
                .ThenInclude(a => a.Subject)
            .FirstOrDefaultAsync(t => t.Id == request.TeacherId && t.TenantId == tenantId, cancellationToken);

        if (teacher is null)
            return Error.NotFound(description: "Docente no encontrado.");

        var assignments = teacher.Assignments
            .Select(a => new TeacherAssignmentSummary(
                AssignmentId: a.Id,
                AcademicYear: a.Section.AcademicYear.Name,
                GradeLevel: a.Section.GradeLevel.Name,
                Section: a.Section.Name,
                Subject: a.Subject.Name,
                Shift: a.Section.Shift.ToString()))
            .ToList();

        return new TeacherDetailResult(
            Id: teacher.Id,
            TenantId: teacher.TenantId,
            SchoolId: teacher.SchoolId,
            SchoolName: teacher.School.Name,
            FirstName: teacher.FirstName,
            LastName: teacher.LastName,
            FullName: teacher.FullName,
            Email: teacher.Email,
            Phone: teacher.Phone,
            NationalId: teacher.NationalId,
            MinerdCode: teacher.MinerdCode,
            TeacherCode: teacher.TeacherCode,
            AcademicTitle: teacher.AcademicTitle,
            Specialization: teacher.Specialization,
            Qualifications: teacher.Qualifications,
            ContractType: teacher.ContractType.ToString(),
            HireDate: teacher.HireDate,
            ContractEndDate: teacher.ContractEndDate,
            WorkingHoursPerWeek: teacher.WorkingHoursPerWeek,
            Gender: teacher.Gender?.ToString(),
            DateOfBirth: teacher.DateOfBirth,
            Address: teacher.Address,
            PhotoUrl: teacher.PhotoUrl,
            IsActive: teacher.IsActive,
            CreatedAt: teacher.CreatedAt,
            CurrentAssignments: assignments);
    }
}
