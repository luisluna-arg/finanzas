using CQRSDispatch;
using Finance.Application.Legacy.Base.Handlers;
using Finance.Application.Legacy.Queries.Base;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Legacy.Queries.CurrencyConvertions;

public class GetAllCurrencyConversionsQuery : GetAllQuery<CurrencyConversion>;

public class GetAllCurrencyConversionsQueryHandler(FinanceDbContext db) : BaseCollectionHandler<GetAllCurrencyConversionsQuery, CurrencyConversion>(db)
{
    public override async Task<DataResult<List<CurrencyConversion>>> ExecuteAsync(GetAllCurrencyConversionsQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.CurrencyConversion.AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        return DataResult<List<CurrencyConversion>>.Success(await query.ToListAsync(cancellationToken));
    }
}
