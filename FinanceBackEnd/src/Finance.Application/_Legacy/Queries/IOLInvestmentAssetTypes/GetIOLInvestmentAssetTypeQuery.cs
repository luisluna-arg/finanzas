using CQRSDispatch;
using Finance.Application.Legacy.Base.Handlers;
using Finance.Application.Legacy.Queries.Base;
using Finance.Application.Repositories;
using Finance.Domain.Enums;
using Finance.Domain.Models.IOLInvestments;
using Finance.Persistence;

namespace Finance.Application.Legacy.Queries.IOLInvestmentAssetTypes;

public class GetIOLInvestmentAssetTypeQuery : GetSingleByIdQuery<IOLInvestmentAssetType?, IOLInvestmentAssetTypeEnum>;

public class GetIOLInvestmentAssetTypeQueryHandler(FinanceDbContext db, IRepository<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum> repository)
    : BaseQueryHandler<GetIOLInvestmentAssetTypeQuery, IOLInvestmentAssetType?>(db)
{
    private readonly IRepository<IOLInvestmentAssetType, IOLInvestmentAssetTypeEnum> _repository = repository;

    public override async Task<DataResult<IOLInvestmentAssetType?>> ExecuteAsync(GetIOLInvestmentAssetTypeQuery request, CancellationToken cancellationToken)
        => DataResult<IOLInvestmentAssetType?>.Success(await _repository.GetByIdAsync(request.Id, cancellationToken));
}
