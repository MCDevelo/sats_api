using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Students;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Students.Commands.CreateStudent;

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, ErrorOr<StudentResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateStudentCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<StudentResult>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var schoolExists = await _db.Schools
            .AnyAsync(s => s.Id == request.SchoolId && s.TenantId == tenantId && s.IsActive, cancellationToken);

        if (!schoolExists)
            return Error.NotFound(description: "Escuela no encontrada.");

        if (request.NationalId is not null)
        {
            var duplicateNid = await _db.Students
                .AnyAsync(s => s.TenantId == tenantId && s.NationalId == request.NationalId, cancellationToken);

            if (duplicateNid)
                return Error.Conflict(description: "Ya existe un estudiante con esa cédula.");
        }

        if (request.Nse is not null)
        {
            var duplicateNse = await _db.Students
                .AnyAsync(s => s.TenantId == tenantId && s.Nse == request.Nse, cancellationToken);

            if (duplicateNse)
                return Error.Conflict(description: "Ya existe un estudiante con ese NSE.");
        }

        if (request.StudentCode is not null)
        {
            var duplicateCode = await _db.Students
                .AnyAsync(s => s.TenantId == tenantId && s.StudentCode == request.StudentCode, cancellationToken);

            if (duplicateCode)
                return Error.Conflict(description: "Ya existe un estudiante con ese código de matrícula.");
        }

        var student = Student.Create(
            tenantId: tenantId,
            schoolId: request.SchoolId,
            firstName: request.FirstName,
            lastName: request.LastName,
            dateOfBirth: request.DateOfBirth,
            gender: request.Gender,
            secondLastName: request.SecondLastName,
            nationalId: request.NationalId,
            nse: request.Nse,
            studentCode: request.StudentCode,
            nationality: request.Nationality,
            address: request.Address,
            province: request.Province,
            municipality: request.Municipality,
            phone: request.Phone,
            bloodType: request.BloodType,
            allergies: request.Allergies,
            medicalNotes: request.MedicalNotes,
            healthInsurance: request.HealthInsurance,
            healthInsuranceNumber: request.HealthInsuranceNumber,
            hasSpecialNeeds: request.HasSpecialNeeds);

        _db.Students.Add(student);
        await _db.SaveChangesAsync(cancellationToken);

        return student.ToResult();
    }
}
