using FinanceApi.Application.Queries.InvestmentAssetIOLs;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.InvestmentAssetIOLs;

public class GetAllInvestmentAssetIOLsQueryHandler : BaseCollectionHandler<GetAllInvestmentAssetIOLsQuery, InvestmentAssetIOL>
{
    public GetAllInvestmentAssetIOLsQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<InvestmentAssetIOL>> Handle(GetAllInvestmentAssetIOLsQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(await DbContext.InvestmentAssetIOLs.ToArrayAsync());
    }
}
