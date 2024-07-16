using FinanceApi.Application.Dtos.Summary;
using FinanceApi.Domain.Models;
using FinanceApi.Domain.Models.Interfaces;
using FinanceApi.Infrastructure.Services;
using FinanceApiApplication.Queries.Summary;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static FinanceApi.Core.Config.DatabaseSeeder;

namespace FinanceApi.Application.Queries.Summary;

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

        var expenses = (await mediator.Send(new GetTotalExpensesQuery())).Items.Sum(e => e.Value);

        var investments = (await mediator.Send(new GetCurrentInvestmentsQuery())).Items.Sum(e => e.Valued);

        var totalFunds = (await mediator.Send(new GetCurrentFundsQuery())).Items
            .Concat((await mediator.Send(new GetCurrentFundsQuery() { DailyUse = true })).Items)
            .Sum(e => e.Value) +
            investments;

        var result = new TotalGeneralSummary(
        [
            new GeneralSummary(Guid.NewGuid().ToString(), "Ingresos", convertedIncomes),
            new GeneralSummary(Guid.NewGuid().ToString(), "Gastos", expenses),
            new GeneralSummary(Guid.NewGuid().ToString(), "Diferencia", convertedIncomes - expenses),
            new GeneralSummary(Guid.NewGuid().ToString(), "Inversiones", investments),
            new GeneralSummary(Guid.NewGuid().ToString(), "Dinero total", totalFunds)
        ]);

        return result;
    }
}

public class GetGeneralSummaryQuery : IRequest<TotalGeneralSummary>
{
    public bool? DailyUse { get; set; }
}
