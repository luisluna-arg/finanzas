using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.IOLInvestmentAssets;

public class GetIOLInvestmentAssetQuery : GetSingleByIdQuery<IOLInvestmentAsset?, Guid>;

public class GetIOLInvestmentAssetQueryHandler(FinanceDbContext db, IRepository<IOLInvestmentAsset, Guid> repository)
    : BaseResponseHandler<GetIOLInvestmentAssetQuery, IOLInvestmentAsset?>(db)
{
    private readonly IRepository<IOLInvestmentAsset, Guid> _repository = repository;

    public override async Task<IOLInvestmentAsset?> Handle(GetIOLInvestmentAssetQuery request, CancellationToken cancellationToken)
        => await _repository.GetByIdAsync(request.Id, cancellationToken);
}
