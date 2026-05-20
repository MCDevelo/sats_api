using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Enrollments.Commands.TransferStudent;

public record TransferStudentCommand(
    Guid EnrollmentId,
    Guid TargetSectionId) : IRequest<ErrorOr<Guid>>;
