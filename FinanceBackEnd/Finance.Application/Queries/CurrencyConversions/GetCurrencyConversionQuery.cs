using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistence;

namespace Finance.Application.Queries.CurrencyConvertions;

public class GetCurrencyConversionQuery : GetSingleByIdQuery<CurrencyConversion?, Guid>;

public class GetCurrencyConversionQueryHandler(FinanceDbContext db, IRepository<CurrencyConversion, Guid> repository)
    : BaseQueryHandler<GetCurrencyConversionQuery, CurrencyConversion?>(db)
{
    private readonly IRepository<CurrencyConversion, Guid> _repository = repository;

    public override async Task<DataResult<CurrencyConversion?>> ExecuteAsync(GetCurrencyConversionQuery request, CancellationToken cancellationToken)
        => DataResult<CurrencyConversion?>.Success(await _repository.GetByIdAsync(request.Id, cancellationToken));
}
