using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Attendance.Commands.UpdateAttendanceRecord;

public class UpdateAttendanceRecordCommandHandler : IRequestHandler<UpdateAttendanceRecordCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateAttendanceRecordCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateAttendanceRecordCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var record = await _db.AttendanceRecords
            .FirstOrDefaultAsync(a => a.Id == request.RecordId && a.TenantId == tenantId, cancellationToken);

        if (record is null)
            return Error.NotFound(description: "Registro de asistencia no encontrado.");

        // Prevent editing records older than 7 days (business rule)
        var daysAgo = DateOnly.FromDateTime(DateTime.UtcNow).DayNumber - record.Date.DayNumber;
        if (daysAgo > 7)
            return Error.Validation(description: "No se puede modificar registros de asistencia con más de 7 días de antigüedad.");

        record.Update(request.Status, request.Notes);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
