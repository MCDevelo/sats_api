using ErrorOr;
using MediatR;
using SchoolERP.Application.Common.Models;

namespace SchoolERP.Application.Tenants.Queries.GetTenants;

public record GetTenantsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    bool? IsActive = null) : IRequest<ErrorOr<PagedResult<TenantResult>>>;

public record TenantResult(
    Guid Id,
    string Name,
    string LegalName,
    string? Rnc,
    string ContactEmail,
    string? ContactPhone,
    string Plan,
    bool IsActive,
    DateTime? TrialEndsAt,
    DateTime ContractStart,
    DateTime? ContractEnd,
    bool OnboardingCompleted,
    int OnboardingStep,
    int SchoolCount,
    DateTime CreatedAt);
