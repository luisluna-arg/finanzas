using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Dtos.Summary;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Summary;

public class GetDebitExpensesQuery : IQuery<Expense>;

public class GetDebitExpensesQueryHandler(FinanceDbContext db) : IQueryHandler<GetDebitExpensesQuery, Expense>
{
    private readonly FinanceDbContext _db = db;

    public async Task<DataResult<Expense>> ExecuteAsync(GetDebitExpensesQuery request, CancellationToken cancellationToken)
    {
        var debitOrigins = _db.DebitOrigin.Where(o => !o.Deactivated).ToArray();
        var debitTotal = 0m;

        foreach (var debitOrigin in debitOrigins)
        {
            var item = await _db.Debit
                .Where(o => o.OriginId == debitOrigin.Id)
                .OrderByDescending(o => o.TimeStamp)
                .FirstOrDefaultAsync();

            debitTotal += item != null ? item.Amount : 0;
        }

        return DataResult<Expense>.Success(new Expense() { Id = "debits", Label = "Débitos", Value = debitTotal });
    }
}
