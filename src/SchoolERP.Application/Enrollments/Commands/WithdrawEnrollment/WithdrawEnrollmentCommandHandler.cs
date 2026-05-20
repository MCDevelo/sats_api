using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Enrollments.Commands.WithdrawEnrollment;

public class WithdrawEnrollmentCommandHandler : IRequestHandler<WithdrawEnrollmentCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public WithdrawEnrollmentCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(WithdrawEnrollmentCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var enrollment = await _db.Enrollments
            .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId && e.TenantId == tenantId, cancellationToken);

        if (enrollment is null)
            return Error.NotFound("Enrollment.NotFound", "Matrícula no encontrada.");

        if (enrollment.Status != EnrollmentStatus.Active)
            return Error.Conflict("Enrollment.NotActive",
                $"La matrícula no está activa (estado actual: {enrollment.Status}).");

        enrollment.Withdraw(request.Reason);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}
