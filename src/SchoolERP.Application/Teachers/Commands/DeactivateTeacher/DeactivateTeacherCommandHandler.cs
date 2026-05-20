using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Teachers.Commands.DeactivateTeacher;

public class DeactivateTeacherCommandHandler : IRequestHandler<DeactivateTeacherCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeactivateTeacherCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(DeactivateTeacherCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var teacher = await _db.Teachers
            .FirstOrDefaultAsync(t => t.Id == request.TeacherId && t.TenantId == tenantId, cancellationToken);

        if (teacher is null)
            return Error.NotFound(description: "Docente no encontrado.");

        if (!teacher.IsActive)
            return Error.Conflict(description: "El docente ya está inactivo.");

        teacher.Deactivate();
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
