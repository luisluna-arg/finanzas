using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;

namespace FinanceApi.Application.Queries.IOLInvestmentAssets;

public class GetIOLInvestmentAssetQueryHandler : BaseResponseHandler<GetIOLInvestmentAssetQuery, IOLInvestmentAsset?>
{
    private readonly IRepository<IOLInvestmentAsset, Guid> investmentAssetIOLTypeRepository;

    public GetIOLInvestmentAssetQueryHandler(
        FinanceDbContext db,
        IRepository<IOLInvestmentAsset, Guid> investmentAssetIOLTypeRepository)
        : base(db)
    {
        this.investmentAssetIOLTypeRepository = investmentAssetIOLTypeRepository;
    }

    public override async Task<IOLInvestmentAsset?> Handle(GetIOLInvestmentAssetQuery request, CancellationToken cancellationToken)
        => await investmentAssetIOLTypeRepository.GetById(request.Id);
}

public class GetIOLInvestmentAssetQuery : GetSingleByIdQuery<IOLInvestmentAsset?, Guid>
{
}
