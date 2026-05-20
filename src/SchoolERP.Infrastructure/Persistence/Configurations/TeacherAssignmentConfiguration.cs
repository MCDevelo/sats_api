using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class TeacherAssignmentConfiguration : IEntityTypeConfiguration<TeacherAssignment>
{
    public void Configure(EntityTypeBuilder<TeacherAssignment> builder)
    {
        builder.ToTable("teacher_assignments");
        builder.HasKey(a => a.Id);

        // A teacher can only have one active assignment per subject+section+year
        builder.HasIndex(a => new { a.TeacherId, a.SectionId, a.SubjectId, a.AcademicYearId })
            .IsUnique()
            .HasFilter("is_active = true");

        // Subject can only be assigned to ONE active teacher per section+year
        builder.HasIndex(a => new { a.SectionId, a.SubjectId, a.AcademicYearId })
            .IsUnique()
            .HasFilter("is_active = true");

        builder.HasIndex(a => a.TeacherId);
        builder.HasIndex(a => a.SectionId);
        builder.HasIndex(a => a.AcademicYearId);

        builder.HasOne(a => a.Teacher)
            .WithMany(t => t.Assignments)
            .HasForeignKey(a => a.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Section)
            .WithMany(s => s.TeacherAssignments)
            .HasForeignKey(a => a.SectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Subject)
            .WithMany(s => s.TeacherAssignments)
            .HasForeignKey(a => a.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.AcademicYear)
            .WithMany()
            .HasForeignKey(a => a.AcademicYearId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
