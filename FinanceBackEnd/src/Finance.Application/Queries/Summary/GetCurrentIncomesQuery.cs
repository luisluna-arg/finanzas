using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Extensions;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models.Incomes;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Summary;

public record GetCurrentIncomesQuery(Guid? BankId = null, Guid? CurrencyId = null) : IQuery<List<Income>>;

public class GetCurrentIncomesQueryHandler(FinanceDbContext db, IRepository<Income, Guid> incomeRepository)
    : BaseQueryHandler<GetCurrentIncomesQuery, List<Income>>(db)
{

    public override async Task<DataResult<List<Income>>> ExecuteAsync(GetCurrentIncomesQuery request, CancellationToken cancellationToken)
    {
        var query = incomeRepository.GetDbSet()
            .Include(o => o.Bank)
            .Include(o => o.Currency)
            .AsQueryable();

        if (request.BankId.HasValue)
        {
            query = query.Where(q => q.BankId == request.BankId.Value);
        }

        if (request.CurrencyId.HasValue)
        {
            query = query.Where(q => q.CurrencyId == request.CurrencyId.Value);
        }

        var dateFilter = DateTime.UtcNow.CurrentMonth().AddMonths(-2);

        var data = await query
            .Where(q => q.TimeStamp >= dateFilter)
            .GroupBy(g => new { g.BankId, g.CurrencyId })
            .Select(g => g.OrderByDescending(i => i.TimeStamp).First())
            .ToListAsync(cancellationToken);

        return DataResult<List<Income>>.Success(data);
    }
}