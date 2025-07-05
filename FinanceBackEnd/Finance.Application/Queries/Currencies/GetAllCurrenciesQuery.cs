using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Currencies;

public class GetAllCurrenciesQuery : GetAllQuery<Currency?>;

public class GetAllCurrenciesQueryHandler(FinanceDbContext db) : BaseCollectionHandler<GetAllCurrenciesQuery, Currency?>(db)
{
    public override async Task<ICollection<Currency?>> Handle(GetAllCurrenciesQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.Currency.AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        return await Task.FromResult(await query.OrderBy(o => o.Name).ToArrayAsync());
    }
}
