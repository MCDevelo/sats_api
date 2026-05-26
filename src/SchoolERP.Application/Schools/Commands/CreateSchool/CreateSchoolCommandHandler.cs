using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Schools.Commands.CreateSchool;

public class CreateSchoolCommandHandler : IRequestHandler<CreateSchoolCommand, ErrorOr<Guid>>
{
    private readonly IApplicationDbContext _db;
    private readonly IPlanService _planService;

    public CreateSchoolCommandHandler(IApplicationDbContext db, IPlanService planService)
    {
        _db = db;
        _planService = planService;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateSchoolCommand request, CancellationToken cancellationToken)
    {
        var tenantExists = await _db.Tenants
            .AnyAsync(t => t.Id == request.TenantId && t.IsActive, cancellationToken);

        if (!tenantExists)
            return Error.NotFound("Tenant.NotFound", "Tenant no encontrado o inactivo.");

        var limits = await _planService.GetLimitsAsync(request.TenantId, cancellationToken);
        if (limits.MaxSchools.HasValue)
        {
            var count = await _db.Schools.CountAsync(s => s.TenantId == request.TenantId && s.IsActive, cancellationToken);
            if (count >= limits.MaxSchools.Value)
                return Error.Unauthorized("Plan.SchoolLimitReached",
                    $"Su plan permite un máximo de {limits.MaxSchools.Value} escuela(s). Actualice su suscripción.");
        }

        if (!string.IsNullOrWhiteSpace(request.CodeMinerd))
        {
            var codeExists = await _db.Schools
                .AnyAsync(s => s.TenantId == request.TenantId && s.CodeMinerd == request.CodeMinerd, cancellationToken);

            if (codeExists)
                return Error.Conflict("School.CodeMinerdTaken", "Ya existe una escuela con ese código MINERD en este tenant.");
        }

        var school = School.Create(
            request.TenantId,
            request.Name,
            request.LevelType,
            request.CodeMinerd,
            request.Email,
            request.PhonePrimary,
            request.Province,
            request.Municipality);

        _db.Schools.Add(school);
        await _db.SaveChangesAsync(cancellationToken);

        return school.Id;
    }
}
