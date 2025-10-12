using Finance.Domain.Models;
using Finance.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Persistence.Configurations;

public class CurrencySymbolConfiguration : IEntityTypeConfiguration<CurrencySymbol>
{
    public void Configure(EntityTypeBuilder<CurrencySymbol> builder)
    {
        builder
            .HasOne(o => o.Currency)
            .WithMany(o => o.Symbols);

        builder
            .HasIndex(o => new { o.Symbol, o.CurrencyId })
            .IsUnique();

        builder
            .HasData(CurrencyConstants.CurrencyIds.SelectMany(o => SymbolMapper(o)).ToList());
    }

    private ICollection<CurrencySymbol> SymbolMapper(string currencyId, bool deactivated = false)
        => CurrencyConstants.CurrencySymbols.ContainsKey(currencyId) ?
        CurrencyConstants.CurrencySymbols[currencyId]
        .Select(o => new CurrencySymbol
        {
            Id = o.Item1,
            CurrencyId = Guid.Parse(currencyId),
            Symbol = o.Item2,
            Deactivated = deactivated
        }).ToArray() : new List<CurrencySymbol>();
}
