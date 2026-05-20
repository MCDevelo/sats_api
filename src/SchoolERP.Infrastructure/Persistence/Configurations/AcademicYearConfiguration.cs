using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class AcademicYearConfiguration : IEntityTypeConfiguration<AcademicYear>
{
    public void Configure(EntityTypeBuilder<AcademicYear> builder)
    {
        builder.ToTable("academic_years");
        builder.HasKey(ay => ay.Id);

        builder.Property(ay => ay.Name).HasMaxLength(20).IsRequired();

        builder.HasIndex(ay => new { ay.TenantId, ay.SchoolId, ay.Name }).IsUnique();
        builder.HasIndex(ay => ay.TenantId);

        builder.HasOne(ay => ay.School)
            .WithMany(s => s.AcademicYears)
            .HasForeignKey(ay => ay.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(ay => ay.Periods)
            .WithOne(p => p.AcademicYear)
            .HasForeignKey(p => p.AcademicYearId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
