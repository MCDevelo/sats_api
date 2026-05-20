using ErrorOr;
using MediatR;

namespace SchoolERP.Application.Enrollments.Commands.WithdrawEnrollment;

public record WithdrawEnrollmentCommand(
    Guid EnrollmentId,
    string Reason) : IRequest<ErrorOr<Success>>;
