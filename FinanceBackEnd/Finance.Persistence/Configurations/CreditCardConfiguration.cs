using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class CreditCardConfiguration : IEntityTypeConfiguration<CreditCard>
{
    public void Configure(EntityTypeBuilder<CreditCard> builder)
    {
        builder
            .HasMany(o => o.Movements)
            .WithOne(o => o.CreditCard)
            .IsRequired();

        builder
            .HasOne(o => o.CreditCardStatement)
            .WithOne(o => o.CreditCard)
            .HasForeignKey<CreditCard>(c => c.CreditCardStatementId);

        builder
            .HasMany(c => c.PaymentPlans)
            .WithOne(p => p.CreditCard)
            .HasForeignKey(p => p.CreditCardId)
            .IsRequired();

        builder
            .HasMany(c => c.Payments)
            .WithOne(p => p.CreditCard)
            .HasForeignKey(p => p.CreditCardId)
            .IsRequired();

        builder
            .Property(c => c.UnappliedCredit)
            .HasColumnType("numeric(18,4)");
    }
}
