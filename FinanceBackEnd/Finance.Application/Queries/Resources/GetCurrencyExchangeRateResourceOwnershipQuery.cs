using Finance.Domain.Models;
using Finance.Persistence;

namespace Finance.Application.Queries.Resources;

public class GetCurrencyExchangeRateOwnershipQuery(Guid id) :
    BaseGetResourceOwnershipQuery<CurrencyExchangeRate, Guid, CurrencyExchangeRateResource>()
{
    public Guid Id { get; } = id;
}

public class GetCurrencyExchangeRateOwnershipQueryHandler : BaseGetResourceOwnershipQueryHandler<GetCurrencyExchangeRateOwnershipQuery, CurrencyExchangeRate, Guid, CurrencyExchangeRateResource>
{
    public GetCurrencyExchangeRateOwnershipQueryHandler(FinanceDbContext dbContext) : base(dbContext)
    {
    }

    protected override IQueryable<CurrencyExchangeRate> SourceQuery(GetCurrencyExchangeRateOwnershipQuery request)
    {
        return DbContext.CurrencyExchangeRate.Where(r => r.Id == request.Id);
    }
}