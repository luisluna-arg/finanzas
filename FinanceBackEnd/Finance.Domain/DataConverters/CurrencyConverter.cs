using Finance.Domain.SpecialTypes;
using Finance.Domain.Models;

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