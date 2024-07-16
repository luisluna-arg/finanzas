using FinanceApi.Core.SpecialTypes;
using FinanceApi.Domain.Models.Base;

namespace FinanceApi.Domain.Models;

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

    public Currency BaseCurrency { get; set; }

    public Currency QuoteCurrency { get; set; }
}
