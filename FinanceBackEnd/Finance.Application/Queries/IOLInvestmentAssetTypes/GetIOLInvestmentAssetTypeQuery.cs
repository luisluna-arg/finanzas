using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Domain.Enums;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.IOLInvestmentAssetTypes;

public class GetIOLInvestmentAssetTypeQuery : GetSingleByIdQuery<IOLInvestmentAssetType?, IOLInvestmentAssetTypeEnum>;

public class GetIOLInvestmentAssetTypeQueryHandler(FinanceDbContext db, IRepository<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum> repository)
    : BaseResponseHandler<GetIOLInvestmentAssetTypeQuery, IOLInvestmentAssetType?>(db)
{
    private readonly IRepository<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum> _repository = repository;

    public override async Task<IOLInvestmentAssetType?> Handle(GetIOLInvestmentAssetTypeQuery request, CancellationToken cancellationToken)
        => await _repository.GetByIdAsync(request.Id, cancellationToken);
}
