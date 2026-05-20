using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class DocumentRequirementConfiguration : IEntityTypeConfiguration<DocumentRequirement>
{
    public void Configure(EntityTypeBuilder<DocumentRequirement> builder)
    {
        builder.ToTable("document_requirements");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name).HasMaxLength(100).IsRequired();
        builder.Property(d => d.Description).HasMaxLength(500);
        builder.Property(d => d.AcceptedFileTypes).HasMaxLength(100).HasDefaultValue("pdf,jpg,jpeg,png");
        builder.Property(d => d.IsRequired).HasDefaultValue(true);
        builder.Property(d => d.IsActive).HasDefaultValue(true);

        builder.HasIndex(d => d.TenantId);
        builder.HasIndex(d => new { d.TenantId, d.SchoolId });

        builder.HasOne(d => d.School)
            .WithMany()
            .HasForeignKey(d => d.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
