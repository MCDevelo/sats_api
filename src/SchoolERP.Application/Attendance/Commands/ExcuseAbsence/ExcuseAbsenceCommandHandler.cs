using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Attendance.Commands.ExcuseAbsence;

public class ExcuseAbsenceCommandHandler : IRequestHandler<ExcuseAbsenceCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public ExcuseAbsenceCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(ExcuseAbsenceCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var record = await _db.AttendanceRecords
            .FirstOrDefaultAsync(a => a.Id == request.RecordId && a.TenantId == tenantId, cancellationToken);

        if (record is null)
            return Error.NotFound(description: "Registro de asistencia no encontrado.");

        if (record.Status != AttendanceStatus.Absent)
            return Error.Validation(description: "Solo se pueden excusar ausencias.");

        record.Update(AttendanceStatus.Excused, request.Reason);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
