using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.IOLInvestmentAssets;

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
