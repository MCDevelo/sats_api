using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class GradeLevelConfiguration : IEntityTypeConfiguration<GradeLevel>
{
    public void Configure(EntityTypeBuilder<GradeLevel> builder)
    {
        builder.ToTable("grade_levels");
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name).HasMaxLength(100).IsRequired();
        builder.Property(g => g.EducationLevel).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(g => g.TenantId);
        builder.HasIndex(g => new { g.SchoolId, g.Order }).IsUnique();
        builder.HasIndex(g => new { g.SchoolId, g.Name }).IsUnique();

        builder.HasOne(g => g.School)
            .WithMany()
            .HasForeignKey(g => g.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
