using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Sections.Commands.AssignHomeTeacher;

public class AssignHomeTeacherCommandHandler : IRequestHandler<AssignHomeTeacherCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public AssignHomeTeacherCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(AssignHomeTeacherCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var section = await _db.Sections
            .FirstOrDefaultAsync(s => s.Id == request.SectionId && s.TenantId == tenantId, cancellationToken);

        if (section is null)
            return Error.NotFound("Section.NotFound", "Sección no encontrada.");

        if (request.TeacherId is null)
        {
            section.RemoveHomeTeacher();
        }
        else
        {
            var teacherExists = await _db.Teachers
                .AnyAsync(t => t.Id == request.TeacherId && t.TenantId == tenantId && t.IsActive, cancellationToken);

            if (!teacherExists)
                return Error.NotFound("Teacher.NotFound", "Docente no encontrado o inactivo.");

            section.AssignHomeTeacher(request.TeacherId.Value);
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}
