using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
{
    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.ToTable("announcements");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Body).HasMaxLength(5000).IsRequired();
        builder.Property(a => a.Audience).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(a => a.Priority).HasConversion<string>().HasMaxLength(10).IsRequired();

        builder.HasIndex(a => new { a.TenantId, a.SchoolId, a.IsPublished });
        builder.HasIndex(a => new { a.SchoolId, a.Audience });

        builder.HasOne(a => a.School)
            .WithMany()
            .HasForeignKey(a => a.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Author)
            .WithMany()
            .HasForeignKey(a => a.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Tenant)
            .WithMany()
            .HasForeignKey(a => a.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
