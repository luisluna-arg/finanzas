using Finance.Domain.Models.Currencies;
using Finance.Domain.SpecialTypes;

namespace Finance.Domain.Policies;

public class CurrencyConversionPolicy : ICurrencyConversionPolicy
{
    public Money Apply(Money amount, Guid sourceCurrencyId, CurrencyExchangeRate currencyExchangeRate)
        => currencyExchangeRate.BaseCurrencyId == sourceCurrencyId
            ? amount / currencyExchangeRate.BuyRate
            : amount * currencyExchangeRate.SellRate;
}

public interface ICurrencyConversionPolicy
{
    Money Apply(Money amount, Guid sourceCurrencyId, CurrencyExchangeRate currencyExchangeRate);
}
