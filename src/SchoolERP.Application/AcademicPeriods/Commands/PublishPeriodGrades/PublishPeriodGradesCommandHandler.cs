using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.AcademicPeriods.Commands.PublishPeriodGrades;

public class PublishPeriodGradesCommandHandler : IRequestHandler<PublishPeriodGradesCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public PublishPeriodGradesCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(PublishPeriodGradesCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var period = await _db.AcademicPeriods
            .Include(p => p.AcademicYear)
            .FirstOrDefaultAsync(p => p.Id == request.AcademicPeriodId
                && p.AcademicYear.TenantId == tenantId, cancellationToken);

        if (period is null)
            return Error.NotFound("AcademicPeriod.NotFound", "Período académico no encontrado.");

        if (period.GradesPublished)
            return Error.Conflict("AcademicPeriod.GradesAlreadyPublished", "Las notas de este período ya han sido publicadas.");

        period.PublishGrades();
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}
