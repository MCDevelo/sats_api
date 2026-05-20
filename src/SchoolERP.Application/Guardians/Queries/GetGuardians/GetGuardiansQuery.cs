using ErrorOr;
using MediatR;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Guardians.Commands.CreateGuardian;

namespace SchoolERP.Application.Guardians.Queries.GetGuardians;

public record GetGuardiansQuery : IRequest<ErrorOr<PagedResult<GuardianResult>>>
{
    public string? Search { get; init; }         // nombre, cédula o email
    public bool? HasPortalAccount { get; init; } // filtra por UserId IS NOT NULL
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
