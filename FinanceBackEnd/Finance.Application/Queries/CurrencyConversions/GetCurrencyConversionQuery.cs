using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.CurrencyConvertions;

public class GetCurrencyConversionQuery : GetSingleByIdQuery<CurrencyConversion?, Guid>;

public class GetCurrencyConversionQueryHandler(FinanceDbContext db, IRepository<CurrencyConversion, Guid> repository)
    : BaseResponseHandler<GetCurrencyConversionQuery, CurrencyConversion?>(db)
{
    private readonly IRepository<CurrencyConversion, Guid> _repository = repository;

    public override async Task<CurrencyConversion?> Handle(GetCurrencyConversionQuery request, CancellationToken cancellationToken)
        => await _repository.GetByIdAsync(request.Id, cancellationToken);
}
