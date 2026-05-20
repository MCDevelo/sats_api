using FluentValidation;

namespace SchoolERP.Application.Enrollments.Commands.WithdrawEnrollment;

public class WithdrawEnrollmentCommandValidator : AbstractValidator<WithdrawEnrollmentCommand>
{
    public WithdrawEnrollmentCommandValidator()
    {
        RuleFor(x => x.EnrollmentId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}
