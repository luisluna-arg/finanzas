using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Base.Handlers;
using Finance.Application.Dtos.Catalog;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Catalog;

public class GetCatalogFrequenciesQuery : IContextAwareQuery<FinanceDispatchContext, List<CatalogIntItemDto>>
{
    internal FinanceDispatchContext Context { get; private set; } = new();
    public void SetContext(FinanceDispatchContext context) => Context = context;
}

public class GetCatalogFrequenciesQueryHandler(FinanceDbContext db) : BaseCollectionQueryHandler<GetCatalogFrequenciesQuery, CatalogIntItemDto>(db)
{
    public override async Task<DataResult<List<CatalogIntItemDto>>> ExecuteAsync(GetCatalogFrequenciesQuery request, CancellationToken cancellationToken)
    {
        var items = await DbContext.Frequency
            .OrderBy(f => f.Name)
            .Select(f => new CatalogIntItemDto { Id = (int)f.Id, Name = f.Name })
            .ToListAsync(cancellationToken);

        return DataResult<List<CatalogIntItemDto>>.Success(items);
    }
}
