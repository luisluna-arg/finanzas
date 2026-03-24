using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Queries.CurrencyExchangeRates;
using Finance.Domain.Policies;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Interfaces;
using Finance.Domain.SpecialTypes;
using Finance.Persistence;
using Microsoft.Extensions.Caching.Memory;

namespace Finance.Application.Services;

public class CurrencyConversionService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext,
    ICurrencyConversionPolicy currencyConversionPolicy,
    IMemoryCache cache)
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public FinanceDbContext _dbContext { get => dbContext; }
    public IDispatcher<FinanceDispatchContext> _dispatcher { get => dispatcher; }

    public async Task<Money> Convert(IAmountHolder value, Guid destinationCurrency)
    {
        if (value.CurrencyId == destinationCurrency)
        {
            return value.Amount;
        }

        var rates = (await GetExchangeRates(new HashSet<Guid> { value.CurrencyId, destinationCurrency })).Data;
        var exchangeRate = rates
            .Where(er =>
                (er.BaseCurrencyId == value.CurrencyId && er.QuoteCurrencyId == destinationCurrency) ||
                (er.BaseCurrencyId == destinationCurrency && er.QuoteCurrencyId == value.CurrencyId))
            .MaxBy(er => er.TimeStamp);
        if (exchangeRate == null)
        {
            return 0;
        }

        return currencyConversionPolicy.Apply(value.Amount, value.CurrencyId, exchangeRate);
    }

    public async Task<ICollection<Money>> ConvertCollection(ICollection<IAmountHolder> values, Guid destinationCurrency)
    {
        var currencyIds = new HashSet<Guid>(values.Select(v => v.CurrencyId)) { destinationCurrency };
        var allRates = await GetExchangeRates(currencyIds);
        var relevantRates = allRates.Data
            .Where(r => r.QuoteCurrencyId == destinationCurrency || r.BaseCurrencyId == destinationCurrency)
            .ToList();

        var result = new List<Money>();
        foreach (var item in values)
        {
            if (item.CurrencyId == destinationCurrency)
            {
                result.Add(item.Amount);
                continue;
            }

            var exchangeRate = relevantRates
                .Where(r =>
                    (r.BaseCurrencyId == item.CurrencyId && r.QuoteCurrencyId == destinationCurrency) ||
                    (r.BaseCurrencyId == destinationCurrency && r.QuoteCurrencyId == item.CurrencyId))
                .MaxBy(r => r.TimeStamp);

            result.Add(exchangeRate != null ?
                currencyConversionPolicy.Apply(item.Amount, item.CurrencyId, exchangeRate) :
                0);
        }

        return result;
    }

    private async Task<DataResult<List<CurrencyExchangeRate>>> GetExchangeRates(HashSet<Guid> currencyIds)
    {
        var accumulated = new List<CurrencyExchangeRate>();
        var missing = new List<Guid>();

        foreach (var id in currencyIds)
        {
            if (cache.TryGetValue($"exchange-rates:{id}", out List<CurrencyExchangeRate>? cached) && cached is not null)
                accumulated.AddRange(cached);
            else
                missing.Add(id);
        }

        if (missing.Count > 0)
        {
            var fetched = (await _dispatcher.DispatchQueryAsync(new GetAllLatestCurrencyExchangeRatesQuery(missing))).Data;

            foreach (var id in missing)
            {
                var ratesForId = fetched.Where(r => r.BaseCurrencyId == id || r.QuoteCurrencyId == id).ToList();
                cache.Set($"exchange-rates:{id}", ratesForId, CacheDuration);
                accumulated.AddRange(ratesForId);
            }
        }

        var distinct = accumulated.DistinctBy(r => new { r.BaseCurrencyId, r.QuoteCurrencyId }).ToList();
        return DataResult<List<CurrencyExchangeRate>>.Success(distinct);
    }
}
