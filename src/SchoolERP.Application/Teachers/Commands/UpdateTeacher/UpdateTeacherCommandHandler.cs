using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Teachers;
using SchoolERP.Application.Teachers.Commands.CreateTeacher;

namespace SchoolERP.Application.Teachers.Commands.UpdateTeacher;

public class UpdateTeacherCommandHandler : IRequestHandler<UpdateTeacherCommand, ErrorOr<TeacherResult>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateTeacherCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<TeacherResult>> Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var teacher = await _db.Teachers
            .FirstOrDefaultAsync(t => t.Id == request.TeacherId && t.TenantId == tenantId, cancellationToken);

        if (teacher is null)
            return Error.NotFound(description: "Docente no encontrado.");

        if (request.Email is not null && request.Email.ToLowerInvariant() != teacher.Email)
        {
            var emailTaken = await _db.Teachers
                .AnyAsync(t => t.TenantId == tenantId && t.Email == request.Email.ToLowerInvariant() && t.Id != teacher.Id, cancellationToken);

            if (emailTaken)
                return Error.Conflict(description: "Ya existe un docente con ese email.");
        }

        teacher.Update(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.Address,
            request.Specialization,
            request.Qualifications,
            request.AcademicTitle,
            request.WorkingHoursPerWeek,
            request.ContractEndDate);

        await _db.SaveChangesAsync(cancellationToken);

        return teacher.ToResult();
    }
}
