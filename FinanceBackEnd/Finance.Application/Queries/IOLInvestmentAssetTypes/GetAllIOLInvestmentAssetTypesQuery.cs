using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.IOLInvestmentAssetTypes;

public class GetAllIOLInvestmentAssetTypesQuery : GetAllQuery<IOLInvestmentAssetType?>;

public class GetAllIOLInvestmentAssetTypesQueryHandler(FinanceDbContext db)
    : BaseCollectionHandler<GetAllIOLInvestmentAssetTypesQuery, IOLInvestmentAssetType?>(db)
{
    public override async Task<ICollection<IOLInvestmentAssetType?>> Handle(GetAllIOLInvestmentAssetTypesQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.IOLInvestmentAssetType.AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        query = query.OrderBy(o => o.Name);

        return await Task.FromResult(await query.ToArrayAsync());
    }
}
