using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("students");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.FirstName).HasMaxLength(60).IsRequired();
        builder.Property(s => s.LastName).HasMaxLength(60).IsRequired();
        builder.Property(s => s.SecondLastName).HasMaxLength(60);
        builder.Property(s => s.NationalId).HasMaxLength(11);
        builder.Property(s => s.Nse).HasMaxLength(20);
        builder.Property(s => s.Nationality).HasMaxLength(60).HasDefaultValue("Dominicana");
        builder.Property(s => s.BloodType).HasMaxLength(3);
        builder.Property(s => s.Allergies).HasMaxLength(1000);
        builder.Property(s => s.MedicalNotes).HasMaxLength(2000);
        builder.Property(s => s.HealthInsurance).HasMaxLength(100);
        builder.Property(s => s.HealthInsuranceNumber).HasMaxLength(30);
        builder.Property(s => s.HasSpecialNeeds).HasDefaultValue(false);
        builder.Property(s => s.Address).HasMaxLength(500);
        builder.Property(s => s.Province).HasMaxLength(100);
        builder.Property(s => s.Municipality).HasMaxLength(100);
        builder.Property(s => s.Phone).HasMaxLength(20);
        builder.Property(s => s.PhotoUrl).HasMaxLength(500);
        builder.Property(s => s.StudentCode).HasMaxLength(20);
        builder.Property(s => s.Gender).HasConversion<string>().HasMaxLength(1);

        builder.HasIndex(s => s.TenantId);
        builder.HasIndex(s => s.SchoolId);
        builder.HasIndex(s => new { s.TenantId, s.StudentCode })
            .IsUnique()
            .HasFilter("\"StudentCode\" IS NOT NULL");
        builder.HasIndex(s => new { s.TenantId, s.NationalId })
            .IsUnique()
            .HasFilter("\"NationalId\" IS NOT NULL");
        builder.HasIndex(s => new { s.TenantId, s.Nse })
            .IsUnique()
            .HasFilter("\"Nse\" IS NOT NULL");

        builder.HasOne(s => s.Tenant)
            .WithMany()
            .HasForeignKey(s => s.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.School)
            .WithMany(sc => sc.Students)
            .HasForeignKey(s => s.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
