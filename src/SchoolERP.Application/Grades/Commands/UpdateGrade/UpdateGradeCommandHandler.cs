using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Grades.Commands.UpdateGrade;

public class UpdateGradeCommandHandler : IRequestHandler<UpdateGradeCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateGradeCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateGradeCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var gradeEntry = await _db.GradeEntries
            .Include(ge => ge.AcademicPeriod)
            .FirstOrDefaultAsync(ge => ge.Id == request.GradeEntryId && ge.TenantId == tenantId, cancellationToken);

        if (gradeEntry is null)
            return Error.NotFound(description: "Calificación no encontrada.");

        if (gradeEntry.IsPublished)
            return Error.Conflict(description: "No se puede modificar una calificación ya publicada.");

        if (gradeEntry.AcademicPeriod.GradesPublished)
            return Error.Conflict(description: "Las calificaciones de este período ya fueron publicadas.");

        gradeEntry.Update(request.Score, request.Comments);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
