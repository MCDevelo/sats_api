using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.ToTable("sections");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).HasMaxLength(10).IsRequired();
        builder.Property(s => s.Shift).HasConversion<string>().HasMaxLength(20);
        builder.Property(s => s.Classroom).HasMaxLength(50);

        builder.HasIndex(s => new { s.GradeLevelId, s.AcademicYearId, s.Name, s.Shift }).IsUnique();
        builder.HasIndex(s => s.TenantId);

        builder.HasOne(s => s.GradeLevel)
            .WithMany(gl => gl.Sections)
            .HasForeignKey(s => s.GradeLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.AcademicYear)
            .WithMany()
            .HasForeignKey(s => s.AcademicYearId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.HomeTeacher)
            .WithMany()
            .HasForeignKey(s => s.HomeTeacherId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}
