using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.IOLInvestmentAssets;

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
        => await investmentAssetIOLTypeRepository.GetByIdAsync(request.Id, cancellationToken);
}

public class GetIOLInvestmentAssetQuery : GetSingleByIdQuery<IOLInvestmentAsset?, Guid>
{
}
