using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Currencies;

public class GetCurrencyQuery : GetSingleByIdQuery<Currency?, Guid>;

public class GetCurrencyQueryHandler(FinanceDbContext db)
    : BaseQueryHandler<GetCurrencyQuery, Currency?>(db)
{
    public override async Task<DataResult<Currency?>> ExecuteAsync(GetCurrencyQuery request, CancellationToken cancellationToken)
    {
        var currency = await DbContext.Currency
            .Include(c => c.Symbols)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        return DataResult<Currency?>.Success(currency);
    }
}
