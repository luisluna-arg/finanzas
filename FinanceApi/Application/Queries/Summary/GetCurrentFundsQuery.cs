using FinanceApi.Application.Dtos.Summary;
using FinanceApi.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.Summary;

public class GetCurrentFundsQueryHandler : IRequestHandler<GetCurrentFundsQuery, TotalFunds>
{
    private readonly FinanceDbContext db;

    public GetCurrentFundsQueryHandler(
        FinanceDbContext db)
    {
        this.db = db;
    }

    public async Task<TotalFunds> Handle(GetCurrentFundsQuery request, CancellationToken cancellationToken)
    {
        var result = new TotalFunds();

        var funds = await db.Fund
            .Include(o => o.Bank)
            .Include(o => o.Currency)
            .Where(o => !o.Deactivated)
            .GroupBy(o => new { o.BankId, o.CurrencyId })
            .Select(o => o.OrderByDescending(x => x.TimeStamp).First())
            .ToArrayAsync(cancellationToken);

        result.Funds.AddRange(funds.Select(o => new Fund($"{o.Id}", $"{o.Bank!.Name} {o.Currency!.Name}", o.Amount)));

        return result;
    }
}

public class GetCurrentFundsQuery : IRequest<TotalFunds>
{
}
