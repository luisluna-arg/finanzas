using FinanceApi.Application.Queries.CurrencyExchangeRates;
using FinanceApi.Core.SpecialTypes;
using FinanceApi.Domain;
using FinanceApi.Domain.DataConverters;
using FinanceApi.Domain.Models;
using FinanceApi.Domain.Models.Interfaces;
using MediatR;

namespace FinanceApi.Infrastructure.Services;

public class CurrencyConversionService(
    IMediator mediator,
    FinanceDbContext dbContext,
    ICurrencyConverter currencyConverter)
{
    public FinanceDbContext DbContext { get => dbContext; }

    public async Task<Money> Convert(IAmountHolder currentIncome, Guid destinationCurrency)
    {
        if (currentIncome.CurrencyId == destinationCurrency)
        {
            return currentIncome.Amount;
        }

        var exchangeRate = (await GetExchangeRates(currentIncome.CurrencyId, destinationCurrency))
            .FirstOrDefault(er => er.BaseCurrencyId == currentIncome.CurrencyId);
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

            var exchangeRate = exchangeRates.FirstOrDefault(er => er.BaseCurrencyId == item.CurrencyId);
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

    private async Task<ICollection<CurrencyExchangeRate>> GetExchangeRates(Guid? baseCurrencyId = default, Guid? quoteCurrencyId = default)
        => await mediator.Send(new GetAllLatestCurrencyExchangeRatesQuery(baseCurrencyId, quoteCurrencyId));
}