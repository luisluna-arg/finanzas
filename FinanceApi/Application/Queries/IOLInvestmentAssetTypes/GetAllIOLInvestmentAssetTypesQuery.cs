using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.IOLInvestmentAssetTypes;

public class GetAllIOLInvestmentAssetTypesQueryHandler : BaseCollectionHandler<GetAllIOLInvestmentAssetTypesQuery, IOLInvestmentAssetType?>
{
    public GetAllIOLInvestmentAssetTypesQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<IOLInvestmentAssetType?>> Handle(GetAllIOLInvestmentAssetTypesQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.IOLInvestmentAssetType.AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        return await Task.FromResult(await query.ToArrayAsync());
    }
}

public class GetAllIOLInvestmentAssetTypesQuery : GetAllQuery<IOLInvestmentAssetType?>
{
}
