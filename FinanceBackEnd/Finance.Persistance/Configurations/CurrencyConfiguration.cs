using Finance.Domain.Models;
using Finance.Persistance.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistance.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder
            .HasKey(o => o.Id);

        builder
            .HasIndex(o => o.Name).IsUnique();

        builder
            .HasIndex(o => o.ShortName).IsUnique();

        builder
            .HasIndex(o => new { o.Name, o.ShortName }).IsUnique();

        builder
            .HasMany(o => o.BaseExchangeRates)
            .WithOne(o => o.BaseCurrency);

        builder
            .HasMany(o => o.QuoteExchangeRates)
            .WithOne(o => o.QuoteCurrency);

        builder
            .HasMany(o => o.Symbols)
            .WithOne(o => o.Currency);

        builder
            .HasData(CurrencyConstants.CurrencyIds.Select(o => CurrencyMapper(o)).ToList());
    }

    private Currency CurrencyMapper(string currencyId, bool deactivated = false)
        => new Currency
        {
            Id = Guid.Parse(currencyId),
            Name = CurrencyConstants.Names.ContainsKey(currencyId) ? CurrencyConstants.Names[currencyId] : string.Empty,
            ShortName = CurrencyConstants.ShortNames.ContainsKey(currencyId) ? CurrencyConstants.ShortNames[currencyId] : string.Empty,
            Deactivated = deactivated
        };
}
