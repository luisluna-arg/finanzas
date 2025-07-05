using Finance.Application.Dtos.Summary;
using MediatR;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Summary;

public class GetDebitExpensesQuery : IRequest<Expense>;

public class GetDebitExpensesQueryHandler(FinanceDbContext db) : IRequestHandler<GetDebitExpensesQuery, Expense>
{
    private readonly FinanceDbContext _db = db;

    public async Task<Expense> Handle(GetDebitExpensesQuery request, CancellationToken cancellationToken)
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

        return new Expense() { Id = "debits", Label = "Débitos", Value = debitTotal };
    }
}
