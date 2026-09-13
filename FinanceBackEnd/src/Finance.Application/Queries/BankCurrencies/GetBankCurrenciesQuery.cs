using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.BankCurrencies;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.BankCurrencies;

public class GetBankCurrenciesQuery : GetAllQuery<BankCurrency>
{
    public bool? DailyUse { get; set; }
}

public class GetBankCurrenciesQueryHandler(FinanceDbContext db) : BaseCollectionQueryHandler<GetBankCurrenciesQuery, BankCurrency>(db)
{
    public override async Task<DataResult<List<BankCurrency>>> ExecuteAsync(GetBankCurrenciesQuery request, CancellationToken cancellationToken = default)
    {
        var query = DbContext.BankCurrency
            .Include(o => o.Bank)
            .Include(o => o.Currency)
            .AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        if (request.DailyUse.HasValue)
        {
            query = query.Where(o => o.DailyUse == request.DailyUse.Value);
        }

        return DataResult<List<BankCurrency>>.Success(await query.ToListAsync(cancellationToken));
    }
}
