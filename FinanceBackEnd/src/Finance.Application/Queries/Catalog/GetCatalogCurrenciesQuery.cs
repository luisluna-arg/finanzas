using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Base.Handlers;
using Finance.Application.Dtos.Catalog;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Catalog;

public class GetCatalogCurrenciesQuery : IContextAwareQuery<FinanceDispatchContext, List<CatalogItemDto>>
{
    internal FinanceDispatchContext Context { get; private set; } = new();
    public void SetContext(FinanceDispatchContext context) => Context = context;
}

public class GetCatalogCurrenciesQueryHandler(FinanceDbContext db) : BaseCollectionQueryHandler<GetCatalogCurrenciesQuery, CatalogItemDto>(db)
{
    public override async Task<DataResult<List<CatalogItemDto>>> ExecuteAsync(GetCatalogCurrenciesQuery request, CancellationToken cancellationToken)
    {
        var currencies = await DbContext.Currency
            .Where(c => !c.Deactivated)
            .OrderBy(c => c.ShortName)
            .Include(c => c.Symbols)
            .ToListAsync(cancellationToken);

        var items = currencies
            .Select(c => new CatalogItemDto { Id = c.Id, Name = c.Symbols.FirstOrDefault()?.Symbol ?? c.ShortName })
            .ToList();

        return DataResult<List<CatalogItemDto>>.Success(items);
    }
}
