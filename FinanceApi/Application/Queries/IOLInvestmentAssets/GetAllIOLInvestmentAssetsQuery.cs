using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;

namespace FinanceApi.Application.Queries.IOLInvestmentAssets;

public class GetAllIOLInvestmentAssetsQueryHandler : BaseCollectionHandler<GetAllIOLInvestmentAssetsQuery, IOLInvestmentAsset>
{
    private readonly IRepository<IOLInvestmentAsset, Guid> investmentAssetIOLTypesRepository;

    public GetAllIOLInvestmentAssetsQueryHandler(
        FinanceDbContext db,
        IRepository<IOLInvestmentAsset, Guid> investmentAssetIOLTypesRepository)
        : base(db)
    {
        this.investmentAssetIOLTypesRepository = investmentAssetIOLTypesRepository;
    }

    public override async Task<ICollection<IOLInvestmentAsset>> Handle(GetAllIOLInvestmentAssetsQuery request, CancellationToken cancellationToken)
        => await investmentAssetIOLTypesRepository.GetAll();
}

public class GetAllIOLInvestmentAssetsQuery : GetAllQuery<IOLInvestmentAsset>
{
}
