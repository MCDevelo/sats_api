using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class StudentGuardianConfiguration : IEntityTypeConfiguration<StudentGuardian>
{
    public void Configure(EntityTypeBuilder<StudentGuardian> builder)
    {
        builder.ToTable("student_guardians");
        builder.HasKey(sg => sg.Id);

        builder.Property(sg => sg.Relationship).HasMaxLength(20).IsRequired();
        builder.Property(sg => sg.CustodyNotes).HasMaxLength(1000);

        // One guardian per student (unique pair)
        builder.HasIndex(sg => new { sg.StudentId, sg.GuardianId }).IsUnique();
        // Fast lookup: all guardians of a student
        builder.HasIndex(sg => sg.StudentId);
        // Fast lookup: all students of a guardian
        builder.HasIndex(sg => sg.GuardianId);

        builder.HasOne(sg => sg.Student)
            .WithMany(s => s.StudentGuardians)
            .HasForeignKey(sg => sg.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sg => sg.Guardian)
            .WithMany(g => g.StudentGuardians)
            .HasForeignKey(sg => sg.GuardianId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
