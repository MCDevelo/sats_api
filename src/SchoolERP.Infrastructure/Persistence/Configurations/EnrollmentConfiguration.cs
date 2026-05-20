using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("enrollments");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.WithdrawalReason).HasMaxLength(500);
        builder.Property(e => e.Notes).HasMaxLength(1000);

        // A student can only be actively enrolled in one section per academic year
        builder.HasIndex(e => new { e.StudentId, e.AcademicYearId })
            .IsUnique()
            .HasFilter("status = 'Active'");

        builder.HasIndex(e => e.TenantId);
        builder.HasIndex(e => e.SectionId);

        builder.HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Section)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.SectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.AcademicYear)
            .WithMany(ay => ay.Enrollments)
            .HasForeignKey(e => e.AcademicYearId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
