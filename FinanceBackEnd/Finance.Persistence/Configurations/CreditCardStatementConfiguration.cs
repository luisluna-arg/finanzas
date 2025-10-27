using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class CreditCardStatementConfiguration : IEntityTypeConfiguration<CreditCardStatement>
{
    public void Configure(EntityTypeBuilder<CreditCardStatement> builder)
    {
        builder.HasKey(s => s.Id);

        builder
            .Property(s => s.ClosureDate)
            .IsRequired()
            .HasConversion(d => d.ToUniversalTime(), d => d);

        builder
            .Property(s => s.ExpiringDate)
            .IsRequired()
            .HasConversion(d => d.ToUniversalTime(), d => d);

        builder
            .Property(s => s.MinimumDue)
            .IsRequired()
            .HasColumnType("numeric(18,4)");

        builder
            .HasOne(s => s.CreditCard)
            .WithMany(c => c.Statements)
            .HasForeignKey(s => s.CreditCardId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(s => new { s.CreditCardId, s.ClosureDate });
    }
}
