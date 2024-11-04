using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.IOLInvestmentAssets;

public class GetAllIOLInvestmentAssetsQueryHandler : BaseCollectionHandler<GetAllIOLInvestmentAssetsQuery, IOLInvestmentAsset?>
{
    public GetAllIOLInvestmentAssetsQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

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

public class GetAllIOLInvestmentAssetsQuery : GetAllQuery<IOLInvestmentAsset?>
{
}
