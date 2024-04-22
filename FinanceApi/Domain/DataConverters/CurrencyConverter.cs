using FinanceApi.Core.SpecialTypes;
using FinanceApi.Domain.Models;

namespace FinanceApi.Domain.DataConverters;

public interface ICurrencyConverter
{
    public Money Convert(Money amount, CurrencyExchangeRate currencyExchangeRate);
}

public class CurrencyConverter : ICurrencyConverter
{
    public Money Convert(Money amount, CurrencyExchangeRate currencyExchangeRate)
        => amount / currencyExchangeRate.SellRate;
}