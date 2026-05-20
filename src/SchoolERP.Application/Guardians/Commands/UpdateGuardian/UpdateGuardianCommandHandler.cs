using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Guardians.Commands.CreateGuardian;

namespace SchoolERP.Application.Guardians.Commands.UpdateGuardian;

public class UpdateGuardianCommandHandler
    : IRequestHandler<UpdateGuardianCommand, ErrorOr<GuardianResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateGuardianCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<GuardianResult>> Handle(
        UpdateGuardianCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var guardian = await _db.Guardians
            .FirstOrDefaultAsync(g => g.Id == request.GuardianId && g.TenantId == tenantId, cancellationToken);

        if (guardian is null)
            return Error.NotFound("Guardian.NotFound", "Encargado no encontrado.");

        guardian.Update(
            firstName: request.FirstName,
            lastName: request.LastName,
            email: request.Email,
            phone: request.Phone,
            phoneSecondary: request.PhoneSecondary,
            whatsApp: request.WhatsApp,
            address: request.Address,
            occupation: request.Occupation,
            workplace: request.Workplace,
            isFinancialResponsible: request.IsFinancialResponsible);

        await _db.SaveChangesAsync(cancellationToken);

        return CreateGuardianCommandHandler.ToResult(guardian);
    }
}
