using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class ScheduleSlotConfiguration : IEntityTypeConfiguration<ScheduleSlot>
{
    public void Configure(EntityTypeBuilder<ScheduleSlot> builder)
    {
        builder.ToTable("schedule_slots");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Day)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(s => s.StartTime).IsRequired();
        builder.Property(s => s.EndTime).IsRequired();
        builder.Property(s => s.Room).HasMaxLength(50);

        // Unique slot per assignment × day × start time
        builder.HasIndex(s => new { s.TeacherAssignmentId, s.Day, s.StartTime }).IsUnique();
        builder.HasIndex(s => s.TenantId);

        builder.HasOne(s => s.TeacherAssignment)
            .WithMany()
            .HasForeignKey(s => s.TeacherAssignmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
