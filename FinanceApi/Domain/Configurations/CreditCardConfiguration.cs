using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceApi.Domain.Configurations;

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
    }
}
