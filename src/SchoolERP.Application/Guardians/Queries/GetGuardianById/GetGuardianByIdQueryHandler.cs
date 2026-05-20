using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Guardians.Queries.GetGuardianById;

public class GetGuardianByIdQueryHandler
    : IRequestHandler<GetGuardianByIdQuery, ErrorOr<GuardianDetailResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetGuardianByIdQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<GuardianDetailResult>> Handle(
        GetGuardianByIdQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var guardian = await _db.Guardians
            .AsNoTracking()
            .Include(g => g.StudentGuardians)
                .ThenInclude(sg => sg.Student)
            .FirstOrDefaultAsync(g => g.Id == request.GuardianId && g.TenantId == tenantId, cancellationToken);

        if (guardian is null)
            return Error.NotFound("Guardian.NotFound", "Encargado no encontrado.");

        var students = guardian.StudentGuardians
            .OrderBy(sg => sg.Student.LastName)
            .ThenBy(sg => sg.Student.FirstName)
            .Select(sg => new GuardianStudentLink(
                StudentGuardianId: sg.Id,
                StudentId: sg.StudentId,
                StudentFullName: sg.Student.FullName,
                StudentCode: sg.Student.StudentCode,
                Relationship: sg.Relationship,
                IsPrimary: sg.IsPrimary,
                CanPickup: sg.CanPickup,
                IsEmergencyContact: sg.IsEmergencyContact,
                ReceivesNotifications: sg.ReceivesNotifications,
                HasCustodyOrder: sg.HasCustodyOrder))
            .ToList();

        return new GuardianDetailResult(
            Id: guardian.Id,
            FirstName: guardian.FirstName,
            LastName: guardian.LastName,
            FullName: guardian.FullName,
            NationalId: guardian.NationalId,
            Email: guardian.Email,
            Phone: guardian.Phone,
            PhoneSecondary: guardian.PhoneSecondary,
            WhatsApp: guardian.WhatsApp,
            Address: guardian.Address,
            Occupation: guardian.Occupation,
            Workplace: guardian.Workplace,
            IsFinancialResponsible: guardian.IsFinancialResponsible,
            Gender: guardian.Gender?.ToString(),
            UserId: guardian.UserId,
            CreatedAt: guardian.CreatedAt,
            Students: students);
    }
}
