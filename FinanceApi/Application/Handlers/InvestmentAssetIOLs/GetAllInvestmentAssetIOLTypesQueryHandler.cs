using FinanceApi.Application.Queries.InvestmentAssetIOLs;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.InvestmentAssetIOLs;

public class GetAllInvestmentAssetIOLTypesQueryHandler : BaseCollectionHandler<GetAllInvestmentAssetIOLTypesQuery, InvestmentAssetIOLType>
{
    public GetAllInvestmentAssetIOLTypesQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<InvestmentAssetIOLType>> Handle(GetAllInvestmentAssetIOLTypesQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(await DbContext.InvestmentAssetIOLTypes.ToArrayAsync());
    }
}
