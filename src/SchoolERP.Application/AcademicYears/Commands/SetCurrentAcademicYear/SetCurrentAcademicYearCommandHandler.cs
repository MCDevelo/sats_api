using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.AcademicYears.Commands.SetCurrentAcademicYear;

public class SetCurrentAcademicYearCommandHandler : IRequestHandler<SetCurrentAcademicYearCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public SetCurrentAcademicYearCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(SetCurrentAcademicYearCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var year = await _db.AcademicYears
            .FirstOrDefaultAsync(y => y.Id == request.AcademicYearId && y.TenantId == tenantId, cancellationToken);

        if (year is null)
            return Error.NotFound("AcademicYear.NotFound", "Año académico no encontrado.");

        // Unset IsCurrent on all other years for this school
        var otherCurrentYears = await _db.AcademicYears
            .Where(y => y.SchoolId == year.SchoolId && y.IsCurrent && y.Id != request.AcademicYearId)
            .ToListAsync(cancellationToken);

        foreach (var other in otherCurrentYears)
            other.Deactivate();

        year.SetAsCurrent();
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}
