using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Dtos.Summary;
using Finance.Persistance;

namespace Finance.Application.Queries.Summary;

public class GetTotalExpensesQuery : IQuery<TotalExpenses>;

public class GetTotalExpensesQueryHandler : IQueryHandler<GetTotalExpensesQuery, TotalExpenses>
{
    private readonly IDispatcher<FinanceDispatchContext> _dispatcher;
    private readonly FinanceDbContext _db;

    public GetTotalExpensesQueryHandler(
        IDispatcher<FinanceDispatchContext> dispatcher,
        FinanceDbContext db)
    {
        _db = db;
        _dispatcher = dispatcher;
    }

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
