using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.IOLInvestmentAssets;

public class GetAllIOLInvestmentAssetsQuery : GetAllQuery<IOLInvestmentAsset>;

public class GetAllIOLInvestmentAssetsQueryHandler(FinanceDbContext db)
    : BaseCollectionHandler<GetAllIOLInvestmentAssetsQuery, IOLInvestmentAsset>(db)
{
    public override async Task<DataResult<List<IOLInvestmentAsset>>> ExecuteAsync(GetAllIOLInvestmentAssetsQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.IOLInvestmentAsset
            .Include(o => o.Type)
            .AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        query = query.OrderBy(o => o.Symbol).ThenBy(o => o.Description).ThenBy(o => o.Type.Name);

        return DataResult<List<IOLInvestmentAsset>>.Success(await query.ToListAsync(cancellationToken));
    }
}
