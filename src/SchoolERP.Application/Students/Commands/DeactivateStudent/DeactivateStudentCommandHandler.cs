using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Students.Commands.DeactivateStudent;

public class DeactivateStudentCommandHandler : IRequestHandler<DeactivateStudentCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeactivateStudentCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(DeactivateStudentCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var student = await _db.Students
            .FirstOrDefaultAsync(s => s.Id == request.StudentId && s.TenantId == tenantId, cancellationToken);

        if (student is null)
            return Error.NotFound(description: "Estudiante no encontrado.");

        if (!student.IsActive)
            return Error.Conflict(description: "El estudiante ya está inactivo.");

        student.Deactivate();
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
