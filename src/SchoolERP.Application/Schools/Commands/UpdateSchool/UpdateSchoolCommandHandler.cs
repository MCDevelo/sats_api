using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Schools.Commands.UpdateSchool;

public class UpdateSchoolCommandHandler : IRequestHandler<UpdateSchoolCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateSchoolCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateSchoolCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId!.Value;

        var school = await _db.Schools
            .FirstOrDefaultAsync(s => s.Id == request.SchoolId && s.TenantId == tenantId, cancellationToken);

        if (school is null)
            return Error.NotFound("School.NotFound", "Escuela no encontrada.");

        school.Update(request.Name, request.Email, request.PhonePrimary, request.Address);
        await _db.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}
