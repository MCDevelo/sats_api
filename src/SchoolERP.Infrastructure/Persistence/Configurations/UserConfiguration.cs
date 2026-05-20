using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email).HasMaxLength(256);
        builder.Property(u => u.Phone).HasMaxLength(20);
        builder.Property(u => u.PasswordHash).HasMaxLength(500);
        builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(50);
        builder.Property(u => u.PreferredLanguage).HasMaxLength(5).HasDefaultValue("es");
        builder.Property(u => u.AvatarUrl).HasMaxLength(500);
        builder.Property(u => u.TwoFactorSecret).HasMaxLength(256);

        builder.HasIndex(u => new { u.TenantId, u.Email }).IsUnique().HasFilter("email IS NOT NULL");
        builder.HasIndex(u => new { u.TenantId, u.Phone }).IsUnique().HasFilter("phone IS NOT NULL");
        builder.HasIndex(u => u.TenantId);

        builder.HasOne(u => u.Tenant)
            .WithMany()
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
