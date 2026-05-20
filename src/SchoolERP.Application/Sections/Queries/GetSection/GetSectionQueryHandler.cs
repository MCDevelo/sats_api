using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Sections.Queries.GetSection;

public class GetSectionQueryHandler : IRequestHandler<GetSectionQuery, ErrorOr<SectionDetailResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetSectionQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<SectionDetailResult>> Handle(GetSectionQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var raw = await _db.Sections
            .Where(s => s.Id == request.SectionId && s.TenantId == tenantId)
            .Select(s => new
            {
                s.Id,
                s.SchoolId,
                s.GradeLevelId,
                GradeLevelName = s.GradeLevel.Name,
                s.AcademicYearId,
                AcademicYearName = s.AcademicYear.Name,
                s.Name,
                s.Shift,
                s.Capacity,
                s.Classroom,
                s.IsActive,
                s.CreatedAt,
                s.HomeTeacherId,
                HomeTeacherName = s.HomeTeacher != null
                    ? s.HomeTeacher.FirstName + " " + s.HomeTeacher.LastName
                    : null,
                EnrolledCount = s.Enrollments.Count(e => e.Status == EnrollmentStatus.Active),
                Students = s.Enrollments
                    .Where(e => e.Status == EnrollmentStatus.Active)
                    .Select(e => new
                    {
                        EnrollmentId = e.Id,
                        e.StudentId,
                        FirstName = e.Student.FirstName,
                        LastName = e.Student.LastName,
                        SecondLastName = e.Student.SecondLastName,
                        e.Student.StudentCode,
                        e.Student.Nse,
                        Gender = e.Student.Gender.ToString(),
                        e.Student.PhotoUrl,
                        e.EnrollmentDate
                    })
                    .ToList(),
                Subjects = s.TeacherAssignments
                    .Where(ta => ta.IsActive)
                    .Select(ta => new
                    {
                        AssignmentId = ta.Id,
                        ta.SubjectId,
                        SubjectName = ta.Subject.Name,
                        TeacherId = (Guid?)ta.TeacherId,
                        TeacherName = ta.Teacher.FirstName + " " + ta.Teacher.LastName
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (raw is null)
            return Error.NotFound("Section.NotFound", "Sección no encontrada.");

        var students = raw.Students
            .Select(s => new SectionStudentSummary(
                s.EnrollmentId,
                s.StudentId,
                string.IsNullOrEmpty(s.SecondLastName)
                    ? s.FirstName + " " + s.LastName
                    : s.FirstName + " " + s.LastName + " " + s.SecondLastName,
                s.StudentCode,
                s.Nse,
                s.Gender,
                s.PhotoUrl,
                s.EnrollmentDate))
            .ToList();

        var subjects = raw.Subjects
            .Select(su => new SectionSubjectSummary(
                su.AssignmentId,
                su.SubjectId,
                su.SubjectName,
                su.TeacherId,
                su.TeacherName))
            .ToList();

        return new SectionDetailResult(
            raw.Id,
            raw.SchoolId,
            raw.GradeLevelId,
            raw.GradeLevelName,
            raw.AcademicYearId,
            raw.AcademicYearName,
            raw.Name,
            raw.Shift.ToString(),
            raw.Capacity,
            raw.EnrolledCount,
            raw.HomeTeacherId,
            raw.HomeTeacherName,
            raw.Classroom,
            raw.IsActive,
            raw.CreatedAt,
            students,
            subjects);
    }
}
