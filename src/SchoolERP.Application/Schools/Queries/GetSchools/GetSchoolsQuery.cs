using ErrorOr;
using MediatR;
using SchoolERP.Application.Common.Models;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Schools.Queries.GetSchools;

public record GetSchoolsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    bool? IsActive = null) : IRequest<ErrorOr<PagedResult<SchoolResult>>>;

public record SchoolResult(
    Guid Id,
    Guid TenantId,
    string Name,
    string? LegalName,
    string? CodeMinerd,
    string? Rnc,
    string? Province,
    string? Municipality,
    string? Address,
    string? PhonePrimary,
    string? Email,
    string? Website,
    string? LogoUrl,
    EducationLevel LevelType,
    bool IsActive,
    DateTime CreatedAt);
