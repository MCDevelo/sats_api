namespace SchoolERP.Domain.Common;

/// <summary>
/// Base para todas las entidades que pertenecen a un tenant.
/// El TenantInterceptor de EF Core asigna TenantId automáticamente en INSERT.
/// </summary>
public abstract class TenantEntity : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid SchoolId { get; set; }
}
