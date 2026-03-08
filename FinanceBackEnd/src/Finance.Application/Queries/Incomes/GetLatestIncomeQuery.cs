using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models.Incomes;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Incomes;

public record GetLatestIncomeQuery(Guid BankId) : IQuery<Income?>;

public class GetLatestIncomeQueryHandler(FinanceDbContext db, IRepository<Income, Guid> movementRepository)
    : BaseQueryHandler<GetLatestIncomeQuery, Income?>(db)
{

    public override async Task<DataResult<Income?>> ExecuteAsync(GetLatestIncomeQuery request, CancellationToken cancellationToken)
    {
        var query = movementRepository.GetDbSet()
            .Include(o => o.Bank)
            .Include(o => o.Currency)
            .AsQueryable();

        return DataResult<Income?>.Success(await query.FirstOrDefaultAsync(o => o.CurrencyId == request.BankId, cancellationToken));
    }
}
