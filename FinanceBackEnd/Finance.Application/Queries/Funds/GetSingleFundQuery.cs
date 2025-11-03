using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models;
using Finance.Persistence;

namespace Finance.Application.Queries.Funds;

public class GetSingleFundQuery : GetSingleByIdQuery<Fund?, Guid>;

public class GetSingleFundQueryHandler(FinanceDbContext db, IRepository<Fund, Guid> repository)
    : BaseQueryHandler<GetSingleFundQuery, Fund?>(db)
{
    private readonly IRepository<Fund, Guid> _repository = repository;

    public override async Task<DataResult<Fund?>> ExecuteAsync(GetSingleFundQuery request, CancellationToken cancellationToken)
        => DataResult<Fund?>.Success(await _repository.GetByIdAsync(request.Id, cancellationToken));
}
