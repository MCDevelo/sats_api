using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Teachers;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Teachers.Commands.CreateTeacher;

public class CreateTeacherCommandHandler : IRequestHandler<CreateTeacherCommand, ErrorOr<TeacherResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateTeacherCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<TeacherResult>> Handle(CreateTeacherCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var schoolExists = await _db.Schools
            .AnyAsync(s => s.Id == request.SchoolId && s.TenantId == tenantId && s.IsActive, cancellationToken);

        if (!schoolExists)
            return Error.NotFound(description: "Escuela no encontrada.");

        if (request.NationalId is not null)
        {
            var duplicate = await _db.Teachers
                .AnyAsync(t => t.TenantId == tenantId && t.NationalId == request.NationalId, cancellationToken);

            if (duplicate)
                return Error.Conflict(description: "Ya existe un docente con esa cédula.");
        }

        if (request.MinerdCode is not null)
        {
            var duplicateMinerd = await _db.Teachers
                .AnyAsync(t => t.TenantId == tenantId && t.MinerdCode == request.MinerdCode, cancellationToken);

            if (duplicateMinerd)
                return Error.Conflict(description: "Ya existe un docente con ese código MINERD.");
        }

        if (request.Email is not null)
        {
            var emailTaken = await _db.Teachers
                .AnyAsync(t => t.TenantId == tenantId && t.Email == request.Email.ToLowerInvariant(), cancellationToken);

            if (emailTaken)
                return Error.Conflict(description: "Ya existe un docente con ese email.");
        }

        var teacher = Teacher.Create(
            tenantId: tenantId,
            schoolId: request.SchoolId,
            firstName: request.FirstName,
            lastName: request.LastName,
            contractType: request.ContractType,
            hireDate: request.HireDate,
            email: request.Email,
            phone: request.Phone,
            nationalId: request.NationalId,
            minerdCode: request.MinerdCode,
            teacherCode: request.TeacherCode,
            academicTitle: request.AcademicTitle,
            specialization: request.Specialization,
            qualifications: request.Qualifications,
            gender: request.Gender,
            dateOfBirth: request.DateOfBirth,
            address: request.Address,
            contractEndDate: request.ContractEndDate,
            workingHoursPerWeek: request.WorkingHoursPerWeek);

        _db.Teachers.Add(teacher);
        await _db.SaveChangesAsync(cancellationToken);

        return teacher.ToResult();
    }
}
