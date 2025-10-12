using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class CreditCardStatementConfiguration : IEntityTypeConfiguration<CreditCardStatement>
{
    public void Configure(EntityTypeBuilder<CreditCardStatement> builder)
    {
        builder
            .HasOne(o => o.CreditCard)
            .WithOne(o => o.CreditCardStatement)
            .HasForeignKey<CreditCardStatement>(c => c.CreditCardId)
            .IsRequired();
    }
}
