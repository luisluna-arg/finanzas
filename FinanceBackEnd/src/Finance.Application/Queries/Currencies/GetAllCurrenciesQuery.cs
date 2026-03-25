using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Currencies;

public class GetAllCurrenciesQuery : GetAllQuery<Currency>;

public class GetAllCurrenciesQueryHandler(FinanceDbContext db) : BaseCollectionQueryHandler<GetAllCurrenciesQuery, Currency>(db)
{
    public override async Task<DataResult<List<Currency>>> ExecuteAsync(GetAllCurrenciesQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.Currency
            .Include(c => c.Symbols)
            .AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        return DataResult<List<Currency>>.Success(await query.OrderBy(o => o.Name).ToListAsync(cancellationToken));
    }
}
