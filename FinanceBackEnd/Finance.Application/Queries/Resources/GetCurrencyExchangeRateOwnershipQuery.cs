using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;

namespace Finance.Application.Queries.Resources;

public class GetCurrencyExchangeRateOwnershipQuery(Guid id) :
    BaseGetResourcePermissionsQuery<CurrencyExchangeRate, Guid, CurrencyExchangeRatePermissions>()
{
    public Guid Id { get; } = id;
}

public class GetCurrencyExchangeRateOwnershipQueryHandler : BaseGetResourcePermissionsQueryHandler<GetCurrencyExchangeRateOwnershipQuery, CurrencyExchangeRate, Guid, CurrencyExchangeRatePermissions>
{
    public GetCurrencyExchangeRateOwnershipQueryHandler(FinanceDbContext dbContext) : base(dbContext)
    {
    }

    protected override IQueryable<CurrencyExchangeRate> SourceQuery(GetCurrencyExchangeRateOwnershipQuery request)
    {
        return DbContext.CurrencyExchangeRate.Where(r => r.Id == request.Id);
    }
}
