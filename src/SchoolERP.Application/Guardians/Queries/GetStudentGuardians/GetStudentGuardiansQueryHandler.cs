using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Guardians.Commands.LinkGuardianToStudent;

namespace SchoolERP.Application.Guardians.Queries.GetStudentGuardians;

public class GetStudentGuardiansQueryHandler
    : IRequestHandler<GetStudentGuardiansQuery, ErrorOr<List<StudentGuardianResult>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetStudentGuardiansQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<List<StudentGuardianResult>>> Handle(
        GetStudentGuardiansQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var studentExists = await _db.Students
            .AnyAsync(s => s.Id == request.StudentId && s.TenantId == tenantId, cancellationToken);

        if (!studentExists)
            return Error.NotFound("Student.NotFound", "Estudiante no encontrado.");

        var links = await _db.StudentGuardians
            .AsNoTracking()
            .Include(sg => sg.Guardian)
            .Include(sg => sg.Student)
            .Where(sg => sg.StudentId == request.StudentId && sg.TenantId == tenantId)
            .OrderByDescending(sg => sg.IsPrimary)
            .ThenBy(sg => sg.Guardian.LastName)
            .Select(sg => new StudentGuardianResult(
                sg.Id,
                sg.StudentId,
                sg.Student.FirstName + " " + sg.Student.LastName,
                sg.GuardianId,
                sg.Guardian.FirstName + " " + sg.Guardian.LastName,
                sg.Guardian.Phone,
                sg.Guardian.Email,
                sg.Guardian.WhatsApp,
                sg.Relationship,
                sg.IsPrimary,
                sg.CanPickup,
                sg.IsEmergencyContact,
                sg.ReceivesNotifications,
                sg.HasCustodyOrder,
                sg.CustodyNotes))
            .ToListAsync(cancellationToken);

        return links;
    }
}
