using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Students;
using SchoolERP.Application.Students.Commands.CreateStudent;

namespace SchoolERP.Application.Students.Commands.UpdateStudent;

public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, ErrorOr<StudentResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateStudentCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<StudentResult>> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var student = await _db.Students
            .FirstOrDefaultAsync(s => s.Id == request.StudentId && s.TenantId == tenantId, cancellationToken);

        if (student is null)
            return Error.NotFound(description: "Estudiante no encontrado.");

        student.Update(
            request.FirstName,
            request.LastName,
            request.SecondLastName,
            request.Address,
            request.Province,
            request.Municipality,
            request.Phone,
            request.MedicalNotes,
            request.Allergies,
            request.HasSpecialNeeds,
            request.HealthInsurance,
            request.HealthInsuranceNumber);

        await _db.SaveChangesAsync(cancellationToken);

        return student.ToResult();
    }
}
