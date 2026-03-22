using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Base.Handlers;
using Finance.Application.Dtos.Catalog;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Catalog;

public class GetCatalogBanksQuery : IContextAwareQuery<FinanceDispatchContext, List<CatalogItemDto>>
{
    internal FinanceDispatchContext Context { get; private set; } = new();
    public void SetContext(FinanceDispatchContext context) => Context = context;
}

public class GetCatalogBanksQueryHandler(FinanceDbContext db) : BaseCollectionQueryHandler<GetCatalogBanksQuery, CatalogItemDto>(db)
{
    public override async Task<DataResult<List<CatalogItemDto>>> ExecuteAsync(GetCatalogBanksQuery request, CancellationToken cancellationToken)
    {
        var items = await DbContext.Bank
            .Where(b => !b.Deactivated)
            .OrderBy(b => b.Name)
            .Select(b => new CatalogItemDto { Id = b.Id, Name = b.Name })
            .ToListAsync(cancellationToken);

        return DataResult<List<CatalogItemDto>>.Success(items);
    }
}
