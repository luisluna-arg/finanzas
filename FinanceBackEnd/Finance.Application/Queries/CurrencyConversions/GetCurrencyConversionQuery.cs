using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Queries.CurrencyConvertions;

public class GetCurrencyConversionQueryHandler : BaseResponseHandler<GetCurrencyConversionQuery, CurrencyConversion?>
{
    private readonly IRepository<CurrencyConversion, Guid> currencyRepository;

    public GetCurrencyConversionQueryHandler(
        FinanceDbContext db,
        IRepository<CurrencyConversion, Guid> currencyRepository)
        : base(db)
    {
        this.currencyRepository = currencyRepository;
    }

    public override async Task<CurrencyConversion?> Handle(GetCurrencyConversionQuery request, CancellationToken cancellationToken)
        => await currencyRepository.GetByIdAsync(request.Id, cancellationToken);
}

public class GetCurrencyConversionQuery : GetSingleByIdQuery<CurrencyConversion?, Guid>
{
}
