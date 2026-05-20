using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Schools.Commands.DeactivateSchool;

public class DeactivateSchoolCommandHandler : IRequestHandler<DeactivateSchoolCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeactivateSchoolCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(DeactivateSchoolCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var school = await _db.Schools
            .FirstOrDefaultAsync(s => s.Id == request.SchoolId && s.TenantId == tenantId, cancellationToken);

        if (school is null)
            return Error.NotFound("School.NotFound", "Escuela no encontrada.");

        if (!school.IsActive)
            return Error.Conflict("School.AlreadyInactive", "La escuela ya está desactivada.");

        school.Deactivate();
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}
