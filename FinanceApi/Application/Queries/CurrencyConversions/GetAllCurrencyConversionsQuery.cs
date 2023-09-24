using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.CurrencyConvertions;

public class GetAllCurrencyConversionsQueryHandler : BaseCollectionHandler<GetAllCurrencyConversionsQuery, CurrencyConversion?>
{
    public GetAllCurrencyConversionsQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<CurrencyConversion?>> Handle(GetAllCurrencyConversionsQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.CurrencyConversion.AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        return await Task.FromResult(await query.ToArrayAsync());
    }
}

public class GetAllCurrencyConversionsQuery : GetAllQuery<CurrencyConversion?>
{
}
