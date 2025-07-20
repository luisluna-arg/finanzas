using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Dtos.Summary;
using Finance.Persistance;
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
            var baseQuery = _db.CreditCardMovement.Where(o => o.CreditCardId == creditCard.Id);
            var count = await baseQuery.CountAsync();
            if (count > 0)
            {
                var timeStamp = await baseQuery.MaxAsync(o => o.TimeStamp);
                total += await baseQuery.Where(o => o.TimeStamp == timeStamp).SumAsync(o => o.Amount);
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
