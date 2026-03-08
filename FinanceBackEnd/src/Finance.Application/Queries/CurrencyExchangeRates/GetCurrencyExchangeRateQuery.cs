using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;

namespace Finance.Application.Queries.CurrencyExchangeRates;

public class GetCurrencyExchangeRateQuery : GetSingleByIdQuery<CurrencyExchangeRate?, Guid>;

public class GetCurrencyExchangeRateQueryHandler(FinanceDbContext db, IRepository<CurrencyExchangeRate, Guid> currencyRepository)
    : BaseQueryHandler<GetCurrencyExchangeRateQuery, CurrencyExchangeRate?>(db)
{
    private readonly IRepository<CurrencyExchangeRate, Guid> _currencyRepository = currencyRepository;

    public override async Task<DataResult<CurrencyExchangeRate?>> ExecuteAsync(GetCurrencyExchangeRateQuery request, CancellationToken cancellationToken)
        => DataResult<CurrencyExchangeRate?>.Success(await _currencyRepository.GetByIdAsync(request.Id, cancellationToken));
}
