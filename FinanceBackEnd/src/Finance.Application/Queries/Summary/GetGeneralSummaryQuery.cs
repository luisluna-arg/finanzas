using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Dtos.Summary;
using Finance.Application.Services;
using Finance.Domain.Models.Incomes;
using Finance.Domain.Models.Interfaces;
using Finance.Persistence.Constants;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Summary;

public record GetGeneralSummaryQuery(bool? DailyUse = null) : IQuery<TotalGeneralSummary>;

public class GetGeneralSummaryQueryHandler(
    IDispatcher<FinanceDispatchContext> dispatcher,
    CurrencyConversionService currencyConverter)
    : IQueryHandler<GetGeneralSummaryQuery, TotalGeneralSummary>
{
    private readonly IDispatcher<FinanceDispatchContext> _dispatcher = dispatcher;
    private readonly CurrencyConversionService _currencyConverterService = currencyConverter;

    public async Task<DataResult<TotalGeneralSummary>> ExecuteAsync(GetGeneralSummaryQuery request, CancellationToken cancellationToken)
    {
        var pesosCurrencyId = Guid.Parse(CurrencyConstants.PesoId);

        var currentIncomesData = await _dispatcher.DispatchQueryAsync(new GetCurrentIncomesQuery());

        IAmountHolder[] currentIncomes = currentIncomesData!.Data.GroupBy(g => g.CurrencyId).Select(g =>
        {
            return new Income() { CurrencyId = g.First().CurrencyId, Amount = g.Sum(a => a.Amount) };
        }).ToArray();

        var convertedIncomes = (await _currencyConverterService.ConvertCollection(currentIncomes!, pesosCurrencyId)).Sum(m => m);

        var investmentItems = (await _dispatcher.DispatchQueryAsync(new GetCurrentInvestmentsQuery())).Data.Items;
        var investmentsConverted = investmentItems.Sum(e => e.ValuedDefaultCurrency);

        var dailyFundItems = (await _dispatcher.DispatchQueryAsync(new GetCurrentFundsQuery() { DailyUse = true })).Data.Items;
        var dailyFundsConverted = dailyFundItems.Sum(e => e.QuoteCurrencyValue);

        var notDailyFundItems = (await _dispatcher.DispatchQueryAsync(new GetCurrentFundsQuery() { DailyUse = false })).Data.Items;
        var notDailyFundsConverted = notDailyFundItems.Sum(e => e.QuoteCurrencyValue);

        var total = dailyFundsConverted + notDailyFundsConverted + investmentsConverted;

        var result = new TotalGeneralSummary(
        [
            new GeneralSummary() { Id = Guid.NewGuid().ToString(), Label = "Ingresos", Value = convertedIncomes, ConvertedValue = convertedIncomes },
            new GeneralSummary() { Id = Guid.NewGuid().ToString(), Label = "Inversiones", Value = 0M, ConvertedValue = investmentsConverted },
            new GeneralSummary() { Id = Guid.NewGuid().ToString(), Label = "Fondos ($)", Value = dailyFundsConverted, ConvertedValue = dailyFundsConverted },
            new GeneralSummary() { Id = Guid.NewGuid().ToString(), Label = "Fondos (U$D / Crypto)", Value = 0M, ConvertedValue = notDailyFundsConverted },
            new GeneralSummary() { Id = Guid.NewGuid().ToString(), Label = "Dinero total", Value = total, ConvertedValue = total }
        ]);

        return DataResult<TotalGeneralSummary>.Success(result);
    }
}
