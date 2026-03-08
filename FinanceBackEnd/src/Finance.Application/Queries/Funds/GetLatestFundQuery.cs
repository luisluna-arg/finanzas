using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models.Funds;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Funds;

public record GetLatestFundQuery(Guid BankId, bool? DailyUse = null) : IQuery<Fund?>;

public class GetLatestFundQueryHandler(FinanceDbContext db, IRepository<Fund, Guid> movementRepository)
    : BaseQueryHandler<GetLatestFundQuery, Fund?>(db)
{

    public override async Task<DataResult<Fund?>> ExecuteAsync(GetLatestFundQuery request, CancellationToken cancellationToken)
    {
        var query = movementRepository.GetDbSet()
            .Include(o => o.Bank)
            .Include(o => o.Currency)
            .Where(o => o.BankId == request.BankId)
            .AsQueryable();

        if (request.DailyUse.HasValue)
        {
            query = query.Where(o => o.DailyUse == request.DailyUse.Value);
        }

        return DataResult<Fund?>.Success(await query
            .OrderByDescending(o => o.TimeStamp)
            .ThenByDescending(o => o.CreatedAt)
            .ThenBy(o => o.Id)
            .FirstOrDefaultAsync(cancellationToken));
    }
}
