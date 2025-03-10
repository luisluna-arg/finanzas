using Finance.Application.Dtos.Summary;
using MediatR;
using Finance.Persistance;

namespace Finance.Application.Queries.Summary;

public class GetTotalExpensesQueryHandler : IRequestHandler<GetTotalExpensesQuery, TotalExpenses>
{
    private readonly IMediator mediator;
    private readonly FinanceDbContext db;

    public GetTotalExpensesQueryHandler(
        IMediator mediator,
        FinanceDbContext db)
    {
        this.db = db;
        this.mediator = mediator;
    }

    public async Task<TotalExpenses> Handle(GetTotalExpensesQuery request, CancellationToken cancellationToken)
    {
        var result = new TotalExpenses();

        result.Add(await mediator.Send(new GetCreditCardExpensesQuery()));

        result.Add(await mediator.Send(new GetDebitExpensesQuery()));

        return result;
    }
}

public class GetTotalExpensesQuery : IRequest<TotalExpenses>
{
}
