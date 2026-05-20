using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Enrollments.Commands.EnrollStudent;

public record EnrollStudentCommand(
    Guid StudentId,
    Guid SectionId,
    DateTime? EnrollmentDate = null,
    string? Notes = null) : IRequest<ErrorOr<Guid>>;
