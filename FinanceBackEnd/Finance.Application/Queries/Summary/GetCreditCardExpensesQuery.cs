using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Dtos.Summary;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Summary;

public class GetCreditCardExpensesQuery : IQuery<Expense>;

public class GetCreditCardExpensesQueryHandler(FinanceDbContext db) : IQueryHandler<GetCreditCardExpensesQuery, Expense>
{
    private readonly FinanceDbContext _db = db;

    public async Task<DataResult<Expense>> ExecuteAsync(GetCreditCardExpensesQuery request, CancellationToken cancellationToken)
    {
        var total = 0;
        var creditCards = _db.CreditCard.Include(o => o.Bank).Where(o => !o.Deactivated).ToArray();
        foreach (var creditCard in creditCards)
        {
            var baseQuery = _db.CreditCardTransaction.Where(o => o.CreditCardId == creditCard.Id);
            var count = await baseQuery.CountAsync(cancellationToken);
            if (count > 0)
            {
                var timeStamp = await baseQuery.MaxAsync(o => o.Timestamp, cancellationToken);
                total += await baseQuery.Where(o => o.Timestamp == timeStamp).SumAsync(o => o.Amount, cancellationToken);
            }
        }

        return DataResult<Expense>.Success(new Expense()
        {
            Id = "creditCards",
            Label = "Tarjetas de crédito",
            Value = total
        });
    }
}
