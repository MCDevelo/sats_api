using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable("subjects");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).HasMaxLength(150).IsRequired();
        builder.Property(s => s.Code).HasMaxLength(20);
        builder.Property(s => s.Description).HasMaxLength(500);

        builder.HasIndex(s => s.GradeLevelId);
        builder.HasIndex(s => s.TenantId);
        builder.HasIndex(s => new { s.TenantId, s.GradeLevelId, s.Code })
            .IsUnique().HasFilter("code IS NOT NULL");

        builder.HasOne(s => s.GradeLevel)
            .WithMany(gl => gl.Subjects)
            .HasForeignKey(s => s.GradeLevelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
