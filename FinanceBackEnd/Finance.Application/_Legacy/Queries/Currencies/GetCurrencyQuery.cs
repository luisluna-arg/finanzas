using CQRSDispatch;
using Finance.Application.Legacy.Base.Handlers;
using Finance.Application.Legacy.Queries.Base;
using Finance.Application.Legacy.Repositories;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;

namespace Finance.Application.Legacy.Queries.Currencies;

public class GetCurrencyQuery : GetSingleByIdQuery<Currency?, Guid>;

public class GetCurrencyQueryHandler(FinanceDbContext db, IRepository<Currency, Guid> repository)
    : BaseQueryHandler<GetCurrencyQuery, Currency?>(db)
{
    private readonly IRepository<Currency, Guid> _repository = repository;

    public override async Task<DataResult<Currency?>> ExecuteAsync(GetCurrencyQuery request, CancellationToken cancellationToken)
        => DataResult<Currency?>.Success(await _repository.GetByIdAsync(request.Id, cancellationToken));
}
