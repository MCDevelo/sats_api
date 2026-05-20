using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.AcademicYears.Commands.CreateAcademicYear;

public class CreateAcademicYearCommandHandler : IRequestHandler<CreateAcademicYearCommand, ErrorOr<Guid>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateAcademicYearCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateAcademicYearCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var schoolExists = await _db.Schools
            .AnyAsync(s => s.Id == request.SchoolId && s.TenantId == tenantId && s.IsActive, cancellationToken);

        if (!schoolExists)
            return Error.NotFound("School.NotFound", "Escuela no encontrada.");

        var nameExists = await _db.AcademicYears
            .AnyAsync(y => y.SchoolId == request.SchoolId && y.Name == request.Name, cancellationToken);

        if (nameExists)
            return Error.Conflict("AcademicYear.NameTaken", "Ya existe un año académico con ese nombre en esta escuela.");

        var year = AcademicYear.Create(tenantId, request.SchoolId, request.Name, request.StartDate, request.EndDate);

        _db.AcademicYears.Add(year);
        await _db.SaveChangesAsync(cancellationToken);

        return year.Id;
    }
}
