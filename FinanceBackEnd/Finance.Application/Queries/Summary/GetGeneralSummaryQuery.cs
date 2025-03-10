using Finance.Application.Dtos.Summary;
using Finance.Domain.Models;
using Finance.Domain.Models.Interfaces;
using Finance.Application.Services;
using Finance.Persistance.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Summary;

public class GetGeneralSummaryQueryHandler : IRequestHandler<GetGeneralSummaryQuery, TotalGeneralSummary>
{
    private readonly IMediator mediator;
    private readonly CurrencyConversionService currencyConverterService;

    public GetGeneralSummaryQueryHandler(
        IMediator mediator,
        CurrencyConversionService currencyConverter)
    {
        this.mediator = mediator;
        this.currencyConverterService = currencyConverter;
    }

    public async Task<TotalGeneralSummary> Handle(GetGeneralSummaryQuery request, CancellationToken cancellationToken)
    {
        var pesosCurrencyId = Guid.Parse(CurrencyConstants.PesoId);

        var currentIncomes = (await mediator.Send(new GetCurrentIncomesQuery())) as ICollection<IAmountHolder>;

        currentIncomes = currentIncomes!.GroupBy(g => g.CurrencyId).Select(g =>
        {
            return new Income() { CurrencyId = g.First().CurrencyId, Amount = g.Sum(a => a.Amount) };
        }).ToArray();

        var convertedIncomes = (await currencyConverterService.ConvertCollection(currentIncomes!, pesosCurrencyId)).Sum(m => m);

        var investments = (await mediator.Send(new GetCurrentInvestmentsQuery())).Items.Sum(e => e.Valued);

        var dailyFunds = (await mediator.Send(new GetCurrentFundsQuery() { DailyUse = true })).Items.Sum(e => e.Value);

        var notDailyFunds = (await mediator.Send(new GetCurrentFundsQuery() { DailyUse = false })).Items.Sum(e => e.Value);

        var totalFunds = dailyFunds + notDailyFunds + investments;

        var result = new TotalGeneralSummary(
        [
            new GeneralSummary(Guid.NewGuid().ToString(), "Ingresos", convertedIncomes),
            new GeneralSummary(Guid.NewGuid().ToString(), "Inversiones", investments),
            new GeneralSummary(Guid.NewGuid().ToString(), "Fondos ($)", dailyFunds),
            new GeneralSummary(Guid.NewGuid().ToString(), "Fondos (U$D / Crypto)", notDailyFunds),
            new GeneralSummary(Guid.NewGuid().ToString(), "Dinero total", totalFunds)
        ]);

        return result;
    }
}

public class GetGeneralSummaryQuery : IRequest<TotalGeneralSummary>
{
    public bool? DailyUse { get; set; }
}
