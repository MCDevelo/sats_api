using ErrorOr;
using MediatR;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Attendance.Commands.UpdateAttendanceRecord;

public record UpdateAttendanceRecordCommand(
    Guid RecordId,
    AttendanceStatus Status,
    string? Notes) : IRequest<ErrorOr<Success>>;
