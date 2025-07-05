using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.IOLInvestmentAssets;

public class GetAllIOLInvestmentAssetsQuery : GetAllQuery<IOLInvestmentAsset?>;

public class GetAllIOLInvestmentAssetsQueryHandler(FinanceDbContext db)
    : BaseCollectionHandler<GetAllIOLInvestmentAssetsQuery, IOLInvestmentAsset?>(db)
{
    public override async Task<ICollection<IOLInvestmentAsset?>> Handle(GetAllIOLInvestmentAssetsQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.IOLInvestmentAsset
            .Include(o => o.Type)
            .AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        query = query.OrderBy(o => o.Symbol).ThenBy(o => o.Description).ThenBy(o => o.Type.Name);

        return await Task.FromResult(await query.ToArrayAsync());
    }
}
