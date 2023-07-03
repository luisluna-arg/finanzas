using FinanceApi.Application.Queries.Currencies;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Currencies;

public class GetAllCurrenciesQueryHandler : BaseCollectionHandler<GetAllCurrenciesQuery, Currency>
{
    public GetAllCurrenciesQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<Currency>> Handle(GetAllCurrenciesQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(await DbContext.Currency.ToArrayAsync());
    }
}
