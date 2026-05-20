using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Guardians.Commands.CreateGuardian;

public class CreateGuardianCommandHandler
    : IRequestHandler<CreateGuardianCommand, ErrorOr<GuardianResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateGuardianCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<GuardianResult>> Handle(
        CreateGuardianCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        // Cédula unique within tenant
        if (request.NationalId is not null)
        {
            var duplicate = await _db.Guardians
                .AnyAsync(g => g.TenantId == tenantId && g.NationalId == request.NationalId, cancellationToken);

            if (duplicate)
                return Error.Conflict("Guardian.DuplicateNationalId",
                    "Ya existe un encargado registrado con esa cédula.");
        }

        var guardian = Guardian.Create(
            tenantId: tenantId,
            firstName: request.FirstName,
            lastName: request.LastName,
            email: request.Email,
            phone: request.Phone,
            nationalId: request.NationalId,
            phoneSecondary: request.PhoneSecondary,
            whatsApp: request.WhatsApp,
            address: request.Address,
            occupation: request.Occupation,
            workplace: request.Workplace,
            isFinancialResponsible: request.IsFinancialResponsible,
            gender: request.Gender);

        _db.Guardians.Add(guardian);
        await _db.SaveChangesAsync(cancellationToken);

        return ToResult(guardian);
    }

    internal static GuardianResult ToResult(Guardian g) => new(
        Id: g.Id,
        FirstName: g.FirstName,
        LastName: g.LastName,
        FullName: g.FullName,
        NationalId: g.NationalId,
        Email: g.Email,
        Phone: g.Phone,
        PhoneSecondary: g.PhoneSecondary,
        WhatsApp: g.WhatsApp,
        Address: g.Address,
        Occupation: g.Occupation,
        Workplace: g.Workplace,
        IsFinancialResponsible: g.IsFinancialResponsible,
        Gender: g.Gender?.ToString(),
        UserId: g.UserId,
        CreatedAt: g.CreatedAt);
}
