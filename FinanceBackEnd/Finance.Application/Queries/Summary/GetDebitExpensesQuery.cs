using Finance.Application.Dtos.Summary;
using MediatR;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Summary;

public class GetDebitExpensesQueryHandler : IRequestHandler<GetDebitExpensesQuery, Expense>
{
    private readonly IMediator mediator;
    private readonly FinanceDbContext db;

    public GetDebitExpensesQueryHandler(
        IMediator mediator,
        FinanceDbContext db)
    {
        this.db = db;
        this.mediator = mediator;
    }

    public async Task<Expense> Handle(GetDebitExpensesQuery request, CancellationToken cancellationToken)
    {
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

        return new Expense("debits", "Débitos", debitTotal);
    }
}

public class GetDebitExpensesQuery : IRequest<Expense>
{
}
