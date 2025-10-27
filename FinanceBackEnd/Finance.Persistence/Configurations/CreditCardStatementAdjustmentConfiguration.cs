using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class CreditCardStatementAdjustmentConfiguration : IEntityTypeConfiguration<CreditCardStatementAdjustment>
{
    public void Configure(EntityTypeBuilder<CreditCardStatementAdjustment> builder)
    {
        builder.HasKey(a => a.Id);

        builder
            .Property(a => a.Amount)
            .IsRequired()
            .HasColumnType("numeric(18,4)");

        builder
            .Property(a => a.Reason)
            .IsRequired()
            .HasMaxLength(500);

        builder
            .Property(a => a.CreatedAt)
            .IsRequired()
            .HasConversion(d => d.ToUniversalTime(), d => d);

        builder
            .HasOne(a => a.CreditCardStatement)
            .WithMany()
            .HasForeignKey(a => a.CreditCardStatementId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(a => new { a.CreditCardStatementId, a.CreatedAt });
    }
}
