using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Teachers.Commands.RemoveAssignment;

public class RemoveAssignmentCommandHandler : IRequestHandler<RemoveAssignmentCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public RemoveAssignmentCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(RemoveAssignmentCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        // Filter via Teacher to enforce tenant scoping (TeacherAssignment has no TenantId)
        var assignment = await _db.TeacherAssignments
            .FirstOrDefaultAsync(a => a.Id == request.AssignmentId
                && a.Teacher.TenantId == tenantId, cancellationToken);

        if (assignment is null)
            return Error.NotFound("Assignment.NotFound", "Asignación no encontrada.");

        if (!assignment.IsActive)
            return Error.Conflict("Assignment.AlreadyInactive", "La asignación ya fue removida.");

        assignment.Deactivate();
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}
