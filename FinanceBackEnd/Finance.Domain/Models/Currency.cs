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

    public virtual ICollection<CurrencyExchangeRate> BaseExchangeRates { get; set; } = new List<CurrencyExchangeRate>();

    public virtual ICollection<CurrencyExchangeRate> QuoteExchangeRates { get; set; } = new List<CurrencyExchangeRate>();

    public static Currency Default()
    {
        return new Currency()
        {
            ShortName = string.Empty,
            Name = string.Empty
        };
    }
}
