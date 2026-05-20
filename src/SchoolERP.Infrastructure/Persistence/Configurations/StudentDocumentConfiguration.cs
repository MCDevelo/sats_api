using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class StudentDocumentConfiguration : IEntityTypeConfiguration<StudentDocument>
{
    public void Configure(EntityTypeBuilder<StudentDocument> builder)
    {
        builder.ToTable("student_documents");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.FileName).HasMaxLength(260).IsRequired();
        builder.Property(d => d.FileUrl).HasMaxLength(500).IsRequired();
        builder.Property(d => d.ContentType).HasMaxLength(100).IsRequired();
        builder.Property(d => d.Notes).HasMaxLength(500);
        builder.Property(d => d.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(d => d.TenantId);
        builder.HasIndex(d => d.StudentId);
        builder.HasIndex(d => new { d.StudentId, d.RequirementId });

        builder.HasOne(d => d.Student)
            .WithMany()
            .HasForeignKey(d => d.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Requirement)
            .WithMany(r => r.StudentDocuments)
            .HasForeignKey(d => d.RequirementId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
