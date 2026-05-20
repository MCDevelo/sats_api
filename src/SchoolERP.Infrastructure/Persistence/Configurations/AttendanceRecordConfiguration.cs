using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.ToTable("attendance_records");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.Notes).HasMaxLength(500);

        // One attendance record per student per day per section
        builder.HasIndex(a => new { a.StudentId, a.SectionId, a.Date }).IsUnique();
        builder.HasIndex(a => a.TenantId);
        builder.HasIndex(a => new { a.SectionId, a.Date });

        builder.HasOne(a => a.Student)
            .WithMany(s => s.AttendanceRecords)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Section)
            .WithMany(s => s.AttendanceRecords)
            .HasForeignKey(a => a.SectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.AcademicPeriod)
            .WithMany(ap => ap.AttendanceRecords)
            .HasForeignKey(a => a.AcademicPeriodId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
