using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Dtos.Summary;
using Finance.Application.Services;
using Finance.Domain.Models;
using Finance.Domain.Models.Interfaces;
using Finance.Persistance.Constants;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Summary;

public class GetGeneralSummaryQueryHandler : IQueryHandler<GetGeneralSummaryQuery, TotalGeneralSummary>
{
    private readonly IDispatcher<FinanceDispatchContext> _dispatcher;
    private readonly CurrencyConversionService _currencyConverterService;

    public GetGeneralSummaryQueryHandler(
        IDispatcher<FinanceDispatchContext> dispatcher,
        CurrencyConversionService currencyConverter)
    {
        _dispatcher = dispatcher;
        _currencyConverterService = currencyConverter;
    }

    public async Task<DataResult<TotalGeneralSummary>> ExecuteAsync(GetGeneralSummaryQuery request, CancellationToken cancellationToken)
    {
        var pesosCurrencyId = Guid.Parse(CurrencyConstants.PesoId);

        var currentIncomesData = await _dispatcher.DispatchQueryAsync(new GetCurrentIncomesQuery());

        IAmountHolder[] currentIncomes = currentIncomesData!.Data.GroupBy(g => g.CurrencyId).Select(g =>
        {
            return new Income() { CurrencyId = g.First().CurrencyId, Amount = g.Sum(a => a.Amount) };
        }).ToArray();

        var convertedIncomes = (await _currencyConverterService.ConvertCollection(currentIncomes!, pesosCurrencyId)).Sum(m => m);

        var investments = (await _dispatcher.DispatchQueryAsync(new GetCurrentInvestmentsQuery())).Data.Items.Sum(e => e.Valued);
        var dailyFunds = (await _dispatcher.DispatchQueryAsync(new GetCurrentFundsQuery() { DailyUse = true })).Data.Items.Sum(e => e.Value);
        var notDailyFunds = (await _dispatcher.DispatchQueryAsync(new GetCurrentFundsQuery() { DailyUse = false })).Data.Items.Sum(e => e.Value);

        var totalFunds = dailyFunds + notDailyFunds + investments;

        var result = new TotalGeneralSummary(
        [
            new GeneralSummary() { Id = Guid.NewGuid().ToString(), Label = "Ingresos", Value = convertedIncomes },
            new GeneralSummary() { Id = Guid.NewGuid().ToString(), Label = "Inversiones", Value = investments },
            new GeneralSummary() { Id = Guid.NewGuid().ToString(), Label = "Fondos ($)", Value = dailyFunds },
            new GeneralSummary() { Id = Guid.NewGuid().ToString(), Label = "Fondos (U$D / Crypto)", Value = notDailyFunds },
            new GeneralSummary() { Id = Guid.NewGuid().ToString(), Label = "Dinero total", Value = totalFunds }
        ]);

        return DataResult<TotalGeneralSummary>.Success(result);
    }
}

public class GetGeneralSummaryQuery : IQuery<TotalGeneralSummary>
{
    public bool? DailyUse { get; set; }
}
