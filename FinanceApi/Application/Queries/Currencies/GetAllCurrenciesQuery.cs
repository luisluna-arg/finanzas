using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.Currencies;

public class GetAllCurrenciesQueryHandler : BaseCollectionHandler<GetAllCurrenciesQuery, Currency?>
{
    public GetAllCurrenciesQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<Currency?>> Handle(GetAllCurrenciesQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.Currency.AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        return await Task.FromResult(await query.OrderBy(o => o.Name).ToArrayAsync());
    }
}

public class GetAllCurrenciesQuery : GetAllQuery<Currency?>
{
}
