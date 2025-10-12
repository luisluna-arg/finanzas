using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Banks;

public class GetAllBanksQuery : GetAllQuery<Bank>;

public class GetAllBanksQueryHandler(FinanceDbContext db) : BaseCollectionHandler<GetAllBanksQuery, Bank>(db)
{
    public override async Task<DataResult<List<Bank>>> ExecuteAsync(GetAllBanksQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.Bank.AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        return DataResult<List<Bank>>.Success(await query.OrderBy(o => o.Name).ToListAsync(cancellationToken));
    }
}
