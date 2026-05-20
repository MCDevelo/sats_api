using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class GradeEntryConfiguration : IEntityTypeConfiguration<GradeEntry>
{
    public void Configure(EntityTypeBuilder<GradeEntry> builder)
    {
        builder.ToTable("grade_entries");
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Score).HasPrecision(5, 2);
        builder.Property(g => g.Comments).HasMaxLength(500);

        // One grade entry per student per evaluation plan
        builder.HasIndex(g => new { g.StudentId, g.EvaluationPlanId }).IsUnique();
        builder.HasIndex(g => g.TenantId);
        builder.HasIndex(g => new { g.SubjectId, g.AcademicPeriodId });

        builder.HasOne(g => g.Student)
            .WithMany(s => s.GradeEntries)
            .HasForeignKey(g => g.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.Subject)
            .WithMany(s => s.GradeEntries)
            .HasForeignKey(g => g.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.AcademicPeriod)
            .WithMany(ap => ap.GradeEntries)
            .HasForeignKey(g => g.AcademicPeriodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(g => g.EvaluationPlan)
            .WithMany(ep => ep.GradeEntries)
            .HasForeignKey(g => g.EvaluationPlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
