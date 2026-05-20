using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Grades.Commands.PublishGrades;

public class PublishGradesCommandHandler : IRequestHandler<PublishGradesCommand, ErrorOr<PublishGradesResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public PublishGradesCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<PublishGradesResult>> Handle(PublishGradesCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var period = await _db.AcademicPeriods
            .FirstOrDefaultAsync(p => p.Id == request.AcademicPeriodId, cancellationToken);

        if (period is null)
            return Error.NotFound(description: "Período académico no encontrado.");

        if (period.GradesPublished)
            return Error.Conflict(description: "Las calificaciones de este período ya fueron publicadas.");

        // Publish all grade entries for this period
        var entries = await _db.GradeEntries
            .Where(ge => ge.TenantId == tenantId && ge.AcademicPeriodId == request.AcademicPeriodId && !ge.IsPublished)
            .ToListAsync(cancellationToken);

        foreach (var entry in entries)
            entry.Publish();

        period.PublishGrades();

        await _db.SaveChangesAsync(cancellationToken);

        return new PublishGradesResult(
            AcademicPeriodId: period.Id,
            PeriodName: period.Name,
            TotalGradeEntries: entries.Count,
            PublishedAt: DateTime.UtcNow);
    }
}
