using ErrorOr;
using MediatR;
using SchoolERP.Application.Common.Models;

namespace SchoolERP.Application.AuditLogs.Queries.GetAuditLogs;

public record GetAuditLogsQuery(
    int Page = 1,
    int PageSize = 50,
    string? EntityName = null,
    string? Action = null,
    string? EntityId = null,
    Guid? UserId = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null) : IRequest<ErrorOr<PagedResult<AuditLogResult>>>;

public record AuditLogResult(
    Guid Id,
    Guid? UserId,
    string Action,
    string EntityName,
    string? EntityId,
    string? OldValues,
    string? NewValues,
    string? IpAddress,
    string? UserAgent,
    DateTime CreatedAt);
