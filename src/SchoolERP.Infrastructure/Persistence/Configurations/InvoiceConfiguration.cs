using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("invoices");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber).HasMaxLength(50).IsRequired();
        builder.Property(i => i.Ncf).HasMaxLength(20);
        builder.Property(i => i.Description).HasMaxLength(500).IsRequired();
        builder.Property(i => i.Amount).HasPrecision(12, 2);
        builder.Property(i => i.AmountPaid).HasPrecision(12, 2);
        builder.Property(i => i.Discount).HasPrecision(12, 2);
        builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(i => i.Notes).HasMaxLength(500);

        builder.HasIndex(i => new { i.TenantId, i.InvoiceNumber }).IsUnique();
        builder.HasIndex(i => i.StudentId);
        builder.HasIndex(i => new { i.TenantId, i.Status });

        builder.HasOne(i => i.Student)
            .WithMany(s => s.Invoices)
            .HasForeignKey(i => i.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.Payments)
            .WithOne(p => p.Invoice)
            .HasForeignKey(p => p.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
