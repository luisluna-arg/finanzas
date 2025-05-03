using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.CurrencyConvertions;

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
