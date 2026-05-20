using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class SchoolConfiguration : IEntityTypeConfiguration<School>
{
    public void Configure(EntityTypeBuilder<School> builder)
    {
        builder.ToTable("schools");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).HasMaxLength(200).IsRequired();
        builder.Property(s => s.LegalName).HasMaxLength(200);
        builder.Property(s => s.CodeMinerd).HasMaxLength(20);
        builder.Property(s => s.Rnc).HasMaxLength(20);
        builder.Property(s => s.Address).HasMaxLength(500);
        builder.Property(s => s.Sector).HasMaxLength(100);
        builder.Property(s => s.Municipality).HasMaxLength(100);
        builder.Property(s => s.Province).HasMaxLength(100);
        builder.Property(s => s.PhonePrimary).HasMaxLength(20);
        builder.Property(s => s.PhoneSecondary).HasMaxLength(20);
        builder.Property(s => s.Email).HasMaxLength(256);
        builder.Property(s => s.Website).HasMaxLength(256);
        builder.Property(s => s.LogoUrl).HasMaxLength(500);
        builder.Property(s => s.SealUrl).HasMaxLength(500);
        builder.Property(s => s.LevelType).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(s => s.TenantId);
        builder.HasIndex(s => new { s.TenantId, s.CodeMinerd }).IsUnique().HasFilter("code_minerd IS NOT NULL");

        builder.HasOne(s => s.Tenant)
            .WithMany(t => t.Schools)
            .HasForeignKey(s => s.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
