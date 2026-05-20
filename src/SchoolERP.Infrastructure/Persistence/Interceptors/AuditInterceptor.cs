using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;
using System.Text.Json;

namespace SchoolERP.Infrastructure.Persistence.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
    // Entity types to skip (high-volume, low-risk, or contain sensitive tokens)
    private static readonly HashSet<Type> ExcludedTypes =
    [
        typeof(AuditLog),
        typeof(RefreshToken),
        typeof(Notification),
        typeof(AttendanceRecord)   // too high-volume
    ];

    // Property names whose values must never appear in the log
    private static readonly HashSet<string> SensitiveProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "PasswordHash", "TokenHash", "HashedToken", "Secret"
    };

    private readonly ICurrentUserService _currentUser;
    private readonly IHttpContextAccessor _http;

    public AuditInterceptor(ICurrentUserService currentUser, IHttpContextAccessor http)
    {
        _currentUser = currentUser;
        _http = http;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            AppendAuditEntries(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    // ── Core logic ───────────────────────────────────────────────────────────

    private void AppendAuditEntries(DbContext context)
    {
        var userId    = _currentUser.IsAuthenticated ? _currentUser.UserId : (Guid?)null;
        var tenantId  = _currentUser.IsAuthenticated ? _currentUser.TenantId : null;
        var ipAddress = _http.HttpContext?.Connection.RemoteIpAddress?.ToString();
        var userAgent = _http.HttpContext?.Request.Headers.UserAgent.ToString();

        var entries = context.ChangeTracker.Entries()
            .Where(e =>
                e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted &&
                !ExcludedTypes.Contains(e.Entity.GetType()))
            .ToList();

        foreach (var entry in entries)
        {
            var auditEntry = BuildAuditEntry(entry, userId, tenantId, ipAddress, userAgent);
            if (auditEntry is not null)
                context.Set<AuditLog>().Add(auditEntry);
        }
    }

    private static AuditLog? BuildAuditEntry(
        EntityEntry entry,
        Guid? userId,
        Guid? tenantId,
        string? ipAddress,
        string? userAgent)
    {
        var entityName = entry.Entity.GetType().Name;
        var entityId   = GetEntityId(entry);

        switch (entry.State)
        {
            case EntityState.Added:
            {
                var newValues = SerializeValues(
                    entry.Properties
                         .Where(p => !SensitiveProperties.Contains(p.Metadata.Name))
                         .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue));

                return AuditLog.Create(
                    action: "CREATE",
                    entityName: entityName,
                    entityId: entityId,
                    tenantId: tenantId,
                    userId: userId,
                    newValues: newValues,
                    ipAddress: ipAddress,
                    userAgent: userAgent);
            }

            case EntityState.Modified:
            {
                var changedProps = entry.Properties
                    .Where(p =>
                        p.IsModified &&
                        !SensitiveProperties.Contains(p.Metadata.Name))
                    .ToList();

                if (changedProps.Count == 0) return null;

                var oldValues = SerializeValues(
                    changedProps.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue));
                var newValues = SerializeValues(
                    changedProps.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue));

                return AuditLog.Create(
                    action: "UPDATE",
                    entityName: entityName,
                    entityId: entityId,
                    tenantId: tenantId,
                    userId: userId,
                    oldValues: oldValues,
                    newValues: newValues,
                    ipAddress: ipAddress,
                    userAgent: userAgent);
            }

            case EntityState.Deleted:
            {
                var oldValues = SerializeValues(
                    entry.Properties
                         .Where(p => !SensitiveProperties.Contains(p.Metadata.Name))
                         .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue));

                return AuditLog.Create(
                    action: "DELETE",
                    entityName: entityName,
                    entityId: entityId,
                    tenantId: tenantId,
                    userId: userId,
                    oldValues: oldValues,
                    ipAddress: ipAddress,
                    userAgent: userAgent);
            }

            default:
                return null;
        }
    }

    private static string? GetEntityId(EntityEntry entry)
    {
        var key = entry.Metadata.FindPrimaryKey();
        if (key is null) return null;

        var values = key.Properties
            .Select(p => entry.Property(p.Name).CurrentValue?.ToString())
            .Where(v => v is not null);

        return string.Join(",", values);
    }

    private static string? SerializeValues(Dictionary<string, object?> values)
    {
        if (values.Count == 0) return null;

        // Convert values to serializable form
        var serializable = values.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value switch
            {
                null => null,
                Enum e => e.ToString(),
                _ => kvp.Value
            });

        return JsonSerializer.Serialize(serializable,
            new JsonSerializerOptions { WriteIndented = false });
    }
}
