using Finance.Domain.Models.Currencies;
using Finance.Domain.SpecialTypes;

namespace Finance.Domain.DataConverters;

public interface ICurrencyConverter
{
    public Money Convert(Money amount, CurrencyExchangeRate currencyExchangeRate);
}

public class CurrencyConverter : ICurrencyConverter
{
    public Money Convert(Money amount, CurrencyExchangeRate currencyExchangeRate)
        => amount / currencyExchangeRate.SellRate;
}
