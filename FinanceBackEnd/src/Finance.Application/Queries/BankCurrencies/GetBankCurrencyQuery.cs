using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Domain.Models.BankCurrencies;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.BankCurrencies;

public record GetBankCurrencyQuery(Guid BankId, Guid CurrencyId) : IQuery<BankCurrency?>;

public class GetBankCurrencyQueryHandler(FinanceDbContext db) : BaseQueryHandler<GetBankCurrencyQuery, BankCurrency?>(db)
{
    public override async Task<DataResult<BankCurrency?>> ExecuteAsync(GetBankCurrencyQuery request, CancellationToken cancellationToken)
    {
        var bankCurrency = await DbContext.BankCurrency
            .Include(o => o.Bank)
            .Include(o => o.Currency)
            .FirstOrDefaultAsync(o => o.BankId == request.BankId && o.CurrencyId == request.CurrencyId, cancellationToken);

        return DataResult<BankCurrency?>.Success(bankCurrency);
    }
}
