using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Guardians.Commands.LinkGuardianToStudent;

public class LinkGuardianToStudentCommandHandler
    : IRequestHandler<LinkGuardianToStudentCommand, ErrorOr<StudentGuardianResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public LinkGuardianToStudentCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<StudentGuardianResult>> Handle(
        LinkGuardianToStudentCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var student = await _db.Students
            .FirstOrDefaultAsync(s => s.Id == request.StudentId && s.TenantId == tenantId, cancellationToken);

        if (student is null)
            return Error.NotFound("Student.NotFound", "Estudiante no encontrado.");

        var guardian = await _db.Guardians
            .FirstOrDefaultAsync(g => g.Id == request.GuardianId && g.TenantId == tenantId, cancellationToken);

        if (guardian is null)
            return Error.NotFound("Guardian.NotFound", "Encargado no encontrado.");

        var alreadyLinked = await _db.StudentGuardians
            .AnyAsync(sg => sg.StudentId == request.StudentId && sg.GuardianId == request.GuardianId, cancellationToken);

        if (alreadyLinked)
            return Error.Conflict("StudentGuardian.AlreadyLinked",
                "El encargado ya está vinculado a este estudiante.");

        // If setting as primary, unset any existing primary for this student
        if (request.IsPrimary)
        {
            var existingPrimary = await _db.StudentGuardians
                .Where(sg => sg.StudentId == request.StudentId && sg.IsPrimary)
                .ToListAsync(cancellationToken);

            foreach (var ep in existingPrimary)
                ep.Update(ep.Relationship, isPrimary: false, ep.CanPickup,
                    ep.IsEmergencyContact, ep.ReceivesNotifications, ep.HasCustodyOrder, ep.CustodyNotes);
        }

        var link = StudentGuardian.Create(
            tenantId: tenantId,
            studentId: request.StudentId,
            guardianId: request.GuardianId,
            relationship: request.Relationship.ToLowerInvariant(),
            isPrimary: request.IsPrimary,
            canPickup: request.CanPickup,
            isEmergencyContact: request.IsEmergencyContact,
            receivesNotifications: request.ReceivesNotifications,
            hasCustodyOrder: request.HasCustodyOrder,
            custodyNotes: request.CustodyNotes);

        _db.StudentGuardians.Add(link);
        await _db.SaveChangesAsync(cancellationToken);

        return ToResult(link, student, guardian);
    }

    internal static StudentGuardianResult ToResult(StudentGuardian sg, Student student, Guardian guardian) => new(
        Id: sg.Id,
        StudentId: sg.StudentId,
        StudentFullName: student.FullName,
        GuardianId: sg.GuardianId,
        GuardianFullName: guardian.FullName,
        GuardianPhone: guardian.Phone,
        GuardianEmail: guardian.Email,
        GuardianWhatsApp: guardian.WhatsApp,
        Relationship: sg.Relationship,
        IsPrimary: sg.IsPrimary,
        CanPickup: sg.CanPickup,
        IsEmergencyContact: sg.IsEmergencyContact,
        ReceivesNotifications: sg.ReceivesNotifications,
        HasCustodyOrder: sg.HasCustodyOrder,
        CustodyNotes: sg.CustodyNotes);
}
