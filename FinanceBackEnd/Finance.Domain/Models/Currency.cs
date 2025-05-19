using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class Currency : Entity<Guid>
{
    public Currency()
        : base()
    {
    }

    required public string Name { get; set; } = string.Empty;

    required public string ShortName { get; set; } = string.Empty;

    public virtual ICollection<CurrencyExchangeRate> BaseExchangeRates { get; set; } = [];

    public virtual ICollection<CurrencyExchangeRate> QuoteExchangeRates { get; set; } = [];

    public virtual ICollection<IOLInvestmentAsset> IOLInvestmentAssets { get; set; } = [];

    public virtual ICollection<CurrencySymbol> Symbols { get; set; } = [];

    public static Currency Default(
        string? shortName = null,
        string? name = null,
        ICollection<CurrencySymbol>? symbols = null
    )
    {
        return new Currency()
        {
            ShortName = shortName ?? string.Empty,
            Name = name ?? string.Empty,
            Symbols = symbols ?? [],
        };
    }
}
