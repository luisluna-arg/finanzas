using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Dtos.Summary;

namespace Finance.Application.Queries.Summary;

public record GetTotalExpensesQuery : IQuery<TotalExpenses>;

public class GetTotalExpensesQueryHandler(IDispatcher<FinanceDispatchContext> dispatcher)
    : IQueryHandler<GetTotalExpensesQuery, TotalExpenses>
{
    private readonly IDispatcher<FinanceDispatchContext> _dispatcher = dispatcher;

    public async Task<DataResult<TotalExpenses>> ExecuteAsync(GetTotalExpensesQuery request, CancellationToken cancellationToken)
    {
        var result = new TotalExpenses();

        var creditCardExpensesResult = await _dispatcher.DispatchQueryAsync<Expense>(new GetCreditCardExpensesQuery());

        result.Add(creditCardExpensesResult.Data);

        var debitExpensesResult = await _dispatcher.DispatchQueryAsync<Expense>(new GetDebitExpensesQuery());

        result.Add(debitExpensesResult.Data);

        return DataResult<TotalExpenses>.Success(result);
    }
}
