using FinanceApi.Application.Dtos.Summary;
using FinanceApi.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.Summary;

public class GetTotalExpensesQueryHandler : IRequestHandler<GetTotalExpensesQuery, TotalExpenses>
{
    private readonly FinanceDbContext db;

    public GetTotalExpensesQueryHandler(
        FinanceDbContext db)
    {
        this.db = db;
    }

    public async Task<TotalExpenses> Handle(GetTotalExpensesQuery request, CancellationToken cancellationToken)
    {
        var result = new TotalExpenses();

        var total = 0;
        var creditCards = db.CreditCard.Include(o => o.Bank).Where(o => !o.Deactivated).ToArray();
        foreach (var creditCard in creditCards)
        {
            var baseQuery = db.CreditCardMovement.Where(o => o.CreditCardId == creditCard.Id);
            var count = await baseQuery.CountAsync();
            if (count > 0)
            {
                var timeStamp = await baseQuery.MaxAsync(o => o.TimeStamp);
                total += await baseQuery.Where(o => o.TimeStamp == timeStamp).SumAsync(o => o.Amount);
            }
        }

        result.Add(new Expense("creditCards", "Tarjetas de crédito", total));

        var debitOrigins = db.DebitOrigin.Where(o => !o.Deactivated).ToArray();
        var debitTotal = 0m;

        foreach (var debitOrigin in debitOrigins)
        {
            var item = await db.Debit
                .Where(o => o.OriginId == debitOrigin.Id)
                .OrderByDescending(o => o.TimeStamp)
                .FirstOrDefaultAsync();

            debitTotal += item != null ? item.Amount : 0;
        }

        result.Add(new Expense("debits", "Débitos", debitTotal));

        return result;
    }
}

public class GetTotalExpensesQuery : IRequest<TotalExpenses>
{
}
