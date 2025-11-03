using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Queries.CurrencyExchangeRates;
using Finance.Domain.DataConverters;
using Finance.Domain.Models;
using Finance.Domain.Models.Interfaces;
using Finance.Domain.SpecialTypes;
using Finance.Persistence;

namespace Finance.Application.Services;

public class CurrencyConversionService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext,
    ICurrencyConverter currencyConverter)
{
    public FinanceDbContext _dbContext { get => dbContext; }
    public IDispatcher<FinanceDispatchContext> _dispatcher { get => dispatcher; }

    public async Task<Money> Convert(IAmountHolder currentIncome, Guid destinationCurrency)
    {
        if (currentIncome.CurrencyId == destinationCurrency)
        {
            return currentIncome.Amount;
        }

        var exchangeRate = (await GetExchangeRates(currentIncome.CurrencyId, destinationCurrency))
            .Data.FirstOrDefault(er => er.BaseCurrencyId == currentIncome.CurrencyId);
        if (exchangeRate == null)
        {
            return 0;
        }

        return currencyConverter.Convert(currentIncome.Amount, exchangeRate);
    }

    public async Task<ICollection<Money>> ConvertCollection(ICollection<IAmountHolder> currentIncomes, Guid destinationCurrency)
    {
        var exchangeRates = await GetExchangeRates(quoteCurrencyId: destinationCurrency);

        var result = new List<Money>();
        foreach (var item in currentIncomes)
        {
            if (item.CurrencyId == destinationCurrency)
            {
                result.Add(item.Amount);
            }

            var exchangeRate = exchangeRates.Data.FirstOrDefault(er => er.BaseCurrencyId == item.CurrencyId);
            if (exchangeRate != null)
            {
                result.Add(currencyConverter.Convert(item.Amount, exchangeRate));
            }
            else
            {
                result.Add(0);
            }
        }

        return result;
    }

    private async Task<DataResult<List<CurrencyExchangeRate>>> GetExchangeRates(Guid? baseCurrencyId = default, Guid? quoteCurrencyId = default)
    {
        var query = new GetAllLatestCurrencyExchangeRatesQuery(baseCurrencyId, quoteCurrencyId);
        var result = await _dispatcher.DispatchQueryAsync<List<CurrencyExchangeRate>>(query);
        return result;
    }
}
