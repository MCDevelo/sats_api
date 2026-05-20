using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.ToTable("teachers");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.FirstName).HasMaxLength(60).IsRequired();
        builder.Property(t => t.LastName).HasMaxLength(60).IsRequired();
        builder.Property(t => t.NationalId).HasMaxLength(11);
        builder.Property(t => t.MinerdCode).HasMaxLength(20);
        builder.Property(t => t.Email).HasMaxLength(255);
        builder.Property(t => t.Phone).HasMaxLength(20);
        builder.Property(t => t.Address).HasMaxLength(500);
        builder.Property(t => t.TeacherCode).HasMaxLength(30);
        builder.Property(t => t.AcademicTitle).HasMaxLength(100);
        builder.Property(t => t.Specialization).HasMaxLength(500);
        builder.Property(t => t.Qualifications).HasMaxLength(1000);
        builder.Property(t => t.PhotoUrl).HasMaxLength(500);
        builder.Property(t => t.WorkingHoursPerWeek).HasDefaultValue(40);
        builder.Property(t => t.ContractType).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Gender).HasConversion<string>().HasMaxLength(1);

        builder.HasIndex(t => t.TenantId);
        builder.HasIndex(t => new { t.TenantId, t.NationalId })
            .IsUnique()
            .HasFilter("national_id IS NOT NULL");
        builder.HasIndex(t => new { t.TenantId, t.Email })
            .IsUnique()
            .HasFilter("email IS NOT NULL");
        builder.HasIndex(t => new { t.TenantId, t.MinerdCode })
            .IsUnique()
            .HasFilter("minerd_code IS NOT NULL");

        builder.HasOne(t => t.Tenant)
            .WithMany()
            .HasForeignKey(t => t.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.School)
            .WithMany(s => s.Teachers)
            .HasForeignKey(t => t.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
