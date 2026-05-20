using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Subjects.Commands.CreateSubject;

public class CreateSubjectCommandHandler : IRequestHandler<CreateSubjectCommand, ErrorOr<Guid>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateSubjectCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var gradeLevel = await _db.GradeLevels
            .FirstOrDefaultAsync(g => g.Id == request.GradeLevelId
                && g.SchoolId == request.SchoolId
                && g.TenantId == tenantId
                && g.IsActive, cancellationToken);

        if (gradeLevel is null)
            return Error.NotFound("GradeLevel.NotFound", "Nivel de grado no encontrado.");

        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            var codeExists = await _db.Subjects
                .AnyAsync(s => s.GradeLevelId == request.GradeLevelId && s.Code == request.Code, cancellationToken);

            if (codeExists)
                return Error.Conflict("Subject.CodeTaken", "Ya existe una materia con ese código en este nivel de grado.");
        }

        var nameExists = await _db.Subjects
            .AnyAsync(s => s.GradeLevelId == request.GradeLevelId
                && s.Name.ToLower() == request.Name.ToLower(), cancellationToken);

        if (nameExists)
            return Error.Conflict("Subject.NameTaken", "Ya existe una materia con ese nombre en este nivel de grado.");

        var subject = Subject.Create(
            tenantId,
            request.SchoolId,
            request.GradeLevelId,
            request.Name,
            request.WeeklyHours,
            request.Code,
            request.IsRequired);

        _db.Subjects.Add(subject);
        await _db.SaveChangesAsync(cancellationToken);

        return subject.Id;
    }
}
