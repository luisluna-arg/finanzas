using FinanceApi.Application.Dtos.Summary;
using FinanceApi.Application.Queries.Incomes;
using FinanceApi.Domain.Models.Interfaces;
using FinanceApi.Infrastructure.Services;
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

        var funds = (await currencyConverterService.ConvertCollection(currentIncomes!, pesosCurrencyId)).Sum(m => m);

        var expenses = (await mediator.Send(new GetTotalExpensesQuery())).Items.Sum(e => e.Value);

        var result = new TotalGeneralSummary(
        [
            new GeneralSummary(Guid.NewGuid().ToString(), "Ingresos", funds),
            new GeneralSummary(Guid.NewGuid().ToString(), "Gastos", expenses),
            new GeneralSummary(Guid.NewGuid().ToString(), "Diferencia", funds - expenses)
        ]);

        return result;
    }
}

public class GetGeneralSummaryQuery : IRequest<TotalGeneralSummary>
{
    public bool? DailyUse { get; set; }
}
