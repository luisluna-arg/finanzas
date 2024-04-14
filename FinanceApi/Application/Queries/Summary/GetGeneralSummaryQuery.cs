using FinanceApi.Application.Dtos.Summary;
using FinanceApi.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.Summary;

public class GetGeneralSummaryQueryHandler : IRequestHandler<GetGeneralSummaryQuery, TotalGeneralSummary>
{
    private readonly IMediator mediator;

    public GetGeneralSummaryQueryHandler(
        IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async Task<TotalGeneralSummary> Handle(GetGeneralSummaryQuery request, CancellationToken cancellationToken)
    {
        var currentFunds = await mediator.Send(new GetCurrentFundsQuery() { DailyUse = request.DailyUse });

        var totalExpenses = await mediator.Send(new GetTotalExpensesQuery());

        var funds = currentFunds.Items.Sum(f => f.Value);

        var expenses = totalExpenses.Items.Sum(e => e.Value);

        var result = new TotalGeneralSummary();

        result.AddRange(new[] {
            new GeneralSummary(Guid.NewGuid().ToString(), "Ingresos", funds),
            new GeneralSummary(Guid.NewGuid().ToString(), "Gastos", expenses),
            new GeneralSummary(Guid.NewGuid().ToString(), "Diferencia", funds - expenses)
            });

        return result;
    }
}

public class GetGeneralSummaryQuery : IRequest<TotalGeneralSummary>
{
    public bool? DailyUse { get; set; }
}
