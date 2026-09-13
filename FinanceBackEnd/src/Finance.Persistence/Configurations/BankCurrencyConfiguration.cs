using Finance.Domain.Models.BankCurrencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class BankCurrencyConfiguration : IEntityTypeConfiguration<BankCurrency>
{
    public void Configure(EntityTypeBuilder<BankCurrency> builder)
    {
        builder.HasKey(o => new { o.BankId, o.CurrencyId });

        builder
            .HasOne(o => o.Bank)
            .WithMany()
            .HasForeignKey(o => o.BankId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(o => o.Currency)
            .WithMany()
            .HasForeignKey(o => o.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
