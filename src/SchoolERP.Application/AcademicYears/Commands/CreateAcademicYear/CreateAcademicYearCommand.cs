using ErrorOr;
using MediatR;

namespace SchoolERP.Application.AcademicYears.Commands.CreateAcademicYear;

public record CreateAcademicYearCommand(
    Guid SchoolId,
    string Name,
    DateTime StartDate,
    DateTime EndDate) : IRequest<ErrorOr<Guid>>;
