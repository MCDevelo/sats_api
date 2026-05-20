using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class EvaluationPlanConfiguration : IEntityTypeConfiguration<EvaluationPlan>
{
    public void Configure(EntityTypeBuilder<EvaluationPlan> builder)
    {
        builder.ToTable("evaluation_plans");
        builder.HasKey(ep => ep.Id);

        builder.Property(ep => ep.Name).HasMaxLength(100).IsRequired();
        builder.Property(ep => ep.Description).HasMaxLength(500);
        builder.Property(ep => ep.Weight).HasPrecision(5, 2);

        builder.HasIndex(ep => new { ep.SubjectId, ep.AcademicPeriodId });
        builder.HasIndex(ep => ep.TenantId);

        builder.HasOne(ep => ep.Subject)
            .WithMany(s => s.EvaluationPlans)
            .HasForeignKey(ep => ep.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ep => ep.AcademicPeriod)
            .WithMany()
            .HasForeignKey(ep => ep.AcademicPeriodId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
