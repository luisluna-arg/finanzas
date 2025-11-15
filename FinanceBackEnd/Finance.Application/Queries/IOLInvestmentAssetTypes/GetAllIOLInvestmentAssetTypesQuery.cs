using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.IOLInvestments;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.IOLInvestmentAssetTypes;

public class GetAllIOLInvestmentAssetTypesQuery : GetAllQuery<IOLInvestmentAssetType>;

public class GetAllIOLInvestmentAssetTypesQueryHandler(FinanceDbContext db)
    : BaseCollectionHandler<GetAllIOLInvestmentAssetTypesQuery, IOLInvestmentAssetType>(db)
{
    public override async Task<DataResult<List<IOLInvestmentAssetType>>> ExecuteAsync(GetAllIOLInvestmentAssetTypesQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.IOLInvestmentAssetType.AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        query = query.OrderBy(o => o.Name);

        return DataResult<List<IOLInvestmentAssetType>>.Success(await query.ToListAsync(cancellationToken));
    }
}
