using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.GradeLevels.Commands.CreateGradeLevel;

public class CreateGradeLevelCommandHandler : IRequestHandler<CreateGradeLevelCommand, ErrorOr<Guid>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateGradeLevelCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateGradeLevelCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var schoolExists = await _db.Schools
            .AnyAsync(s => s.Id == request.SchoolId && s.TenantId == tenantId && s.IsActive, cancellationToken);

        if (!schoolExists)
            return Error.NotFound("School.NotFound", "Escuela no encontrada.");

        var orderExists = await _db.GradeLevels
            .AnyAsync(g => g.SchoolId == request.SchoolId && g.Order == request.Order, cancellationToken);

        if (orderExists)
            return Error.Conflict("GradeLevel.OrderTaken", $"Ya existe un nivel de grado con orden {request.Order} en esta escuela.");

        var gradeLevel = GradeLevel.Create(tenantId, request.SchoolId, request.Name, request.Order, request.EducationLevel);

        _db.GradeLevels.Add(gradeLevel);
        await _db.SaveChangesAsync(cancellationToken);

        return gradeLevel.Id;
    }
}
