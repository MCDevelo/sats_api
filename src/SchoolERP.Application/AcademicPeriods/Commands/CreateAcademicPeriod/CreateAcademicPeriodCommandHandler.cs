using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.AcademicPeriods.Commands.CreateAcademicPeriod;

public class CreateAcademicPeriodCommandHandler : IRequestHandler<CreateAcademicPeriodCommand, ErrorOr<Guid>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateAcademicPeriodCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateAcademicPeriodCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var yearExists = await _db.AcademicYears
            .AnyAsync(y => y.Id == request.AcademicYearId && y.TenantId == tenantId, cancellationToken);

        if (!yearExists)
            return Error.NotFound("AcademicYear.NotFound", "Año académico no encontrado.");

        var periodNumberExists = await _db.AcademicPeriods
            .AnyAsync(p => p.AcademicYearId == request.AcademicYearId && p.PeriodNumber == request.PeriodNumber, cancellationToken);

        if (periodNumberExists)
            return Error.Conflict("AcademicPeriod.NumberTaken", $"Ya existe un período con número {request.PeriodNumber} en este año académico.");

        var period = AcademicPeriod.Create(
            request.AcademicYearId,
            request.Name,
            request.PeriodNumber,
            request.StartDate,
            request.EndDate);

        _db.AcademicPeriods.Add(period);
        await _db.SaveChangesAsync(cancellationToken);

        return period.Id;
    }
}
