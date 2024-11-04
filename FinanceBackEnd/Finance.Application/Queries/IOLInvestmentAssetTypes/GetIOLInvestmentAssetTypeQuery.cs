using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.IOLInvestmentAssetTypes;

public class GetIOLInvestmentAssetTypeQueryHandler : BaseResponseHandler<GetIOLInvestmentAssetTypeQuery, IOLInvestmentAssetType?>
{
    private readonly IRepository<IOLInvestmentAssetType, ushort> investmentAssetIOLTypeRepository;

    public GetIOLInvestmentAssetTypeQueryHandler(
        FinanceDbContext db,
        IRepository<IOLInvestmentAssetType, ushort> investmentAssetIOLTypeRepository)
        : base(db)
    {
        this.investmentAssetIOLTypeRepository = investmentAssetIOLTypeRepository;
    }

    public override async Task<IOLInvestmentAssetType?> Handle(GetIOLInvestmentAssetTypeQuery request, CancellationToken cancellationToken)
        => await investmentAssetIOLTypeRepository.GetByIdAsync(request.Id, cancellationToken);
}

public class GetIOLInvestmentAssetTypeQuery : GetSingleByIdQuery<IOLInvestmentAssetType?, ushort>
{
}
