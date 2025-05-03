using Finance.Domain.SpecialTypes;
using Finance.Domain.Models.Base;

namespace Finance.Domain.Models;

public class CurrencyExchangeRate : Entity<Guid>
{
    public CurrencyExchangeRate()
        : base()
    {
    }

    public Guid BaseCurrencyId { get; set; }

    public Guid QuoteCurrencyId { get; set; }

    public Money BuyRate { get; set; } = 0m;

    public Money SellRate { get; set; } = 0m;

    public DateTime TimeStamp { get; set; }

    public Currency BaseCurrency { get; set; } = default!;

    public Currency QuoteCurrency { get; set; } = default!;
}
