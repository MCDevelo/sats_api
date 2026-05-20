using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class GuardianConfiguration : IEntityTypeConfiguration<Guardian>
{
    public void Configure(EntityTypeBuilder<Guardian> builder)
    {
        builder.ToTable("guardians");
        builder.HasKey(g => g.Id);

        builder.Property(g => g.FirstName).HasMaxLength(60).IsRequired();
        builder.Property(g => g.LastName).HasMaxLength(60).IsRequired();
        builder.Property(g => g.NationalId).HasMaxLength(11);
        builder.Property(g => g.Email).HasMaxLength(255);
        builder.Property(g => g.Phone).HasMaxLength(20);
        builder.Property(g => g.PhoneSecondary).HasMaxLength(20);
        builder.Property(g => g.WhatsApp).HasMaxLength(20);
        builder.Property(g => g.Address).HasMaxLength(500);
        builder.Property(g => g.Occupation).HasMaxLength(100);
        builder.Property(g => g.Workplace).HasMaxLength(150);
        builder.Property(g => g.Gender).HasConversion<string>().HasMaxLength(1);

        builder.HasIndex(g => g.TenantId);
        builder.HasIndex(g => g.UserId);
        // NationalId unique within tenant (nullable — no filtered index needed, EF null exclusion default)
        builder.HasIndex(g => new { g.TenantId, g.NationalId })
            .IsUnique()
            .HasFilter("\"NationalId\" IS NOT NULL");

        builder.HasOne(g => g.Tenant)
            .WithMany()
            .HasForeignKey(g => g.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(g => g.User)
            .WithMany()
            .HasForeignKey(g => g.UserId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}
