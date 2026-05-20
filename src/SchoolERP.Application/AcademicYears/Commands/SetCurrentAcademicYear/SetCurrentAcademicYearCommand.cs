using ErrorOr;
using MediatR;

namespace SchoolERP.Application.AcademicYears.Commands.SetCurrentAcademicYear;

public record SetCurrentAcademicYearCommand(Guid AcademicYearId) : IRequest<ErrorOr<Success>>;
