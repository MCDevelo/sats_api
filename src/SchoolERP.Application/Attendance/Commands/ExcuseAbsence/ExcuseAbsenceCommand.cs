using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Attendance.Commands.ExcuseAbsence;

public record ExcuseAbsenceCommand(
    Guid RecordId,
    string Reason) : IRequest<ErrorOr<Success>>;
