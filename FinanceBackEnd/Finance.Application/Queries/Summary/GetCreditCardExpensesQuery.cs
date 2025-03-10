using Finance.Application.Dtos.Summary;
using MediatR;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Summary;

public class GetCreditCardExpensesQueryHandler : IRequestHandler<GetCreditCardExpensesQuery, Expense>
{
    private readonly FinanceDbContext db;

    public GetCreditCardExpensesQueryHandler(
        IMediator mediator,
        FinanceDbContext db)
    {
        this.db = db;
    }

    public async Task<Expense> Handle(GetCreditCardExpensesQuery request, CancellationToken cancellationToken)
    {
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

        return new Expense("creditCards", "Tarjetas de crédito", total);
    }
}

public class GetCreditCardExpensesQuery : IRequest<Expense>
{
}
