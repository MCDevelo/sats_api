using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).HasMaxLength(200).IsRequired();
        builder.Property(t => t.LegalName).HasMaxLength(200).IsRequired();
        builder.Property(t => t.ContactEmail).HasMaxLength(256).IsRequired();
        builder.Property(t => t.ContactPhone).HasMaxLength(20);
        builder.Property(t => t.Rnc).HasMaxLength(20);
        builder.Property(t => t.Country).HasMaxLength(2).HasDefaultValue("DO");
        builder.Property(t => t.Plan).HasMaxLength(50).HasDefaultValue("trial");
        builder.Property(t => t.LogoUrl).HasMaxLength(500);
        builder.Property(t => t.PrimaryColor).HasMaxLength(10);

        builder.HasIndex(t => t.ContactEmail).IsUnique();
    }
}
