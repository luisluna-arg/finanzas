using CQRSDispatch;
using Finance.Application.Legacy.Base.Handlers;
using Finance.Application.Legacy.Queries.Base;
using Finance.Application.Legacy.Repositories;
using Finance.Domain.Models.IOLInvestments;
using Finance.Persistence;

namespace Finance.Application.Legacy.Queries.IOLInvestmentAssets;

public class GetIOLInvestmentAssetQuery : GetSingleByIdQuery<IOLInvestmentAsset?, Guid>;

public class GetIOLInvestmentAssetQueryHandler(FinanceDbContext db, IRepository<IOLInvestmentAsset, Guid> repository)
    : BaseQueryHandler<GetIOLInvestmentAssetQuery, IOLInvestmentAsset?>(db)
{
    private readonly IRepository<IOLInvestmentAsset, Guid> _repository = repository;

    public override async Task<DataResult<IOLInvestmentAsset?>> ExecuteAsync(GetIOLInvestmentAssetQuery request, CancellationToken cancellationToken)
        => DataResult<IOLInvestmentAsset?>.Success(await _repository.GetByIdAsync(request.Id, cancellationToken));
}
