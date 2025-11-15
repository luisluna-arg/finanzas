using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.Debits;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.DebitOrigins;

public class GetAllDebitOriginsQuery : GetAllQuery<DebitOrigin>;

public class GetAllDebitOriginsQueryHandler(FinanceDbContext db) : BaseCollectionHandler<GetAllDebitOriginsQuery, DebitOrigin>(db)
{
    public override async Task<DataResult<List<DebitOrigin>>> ExecuteAsync(GetAllDebitOriginsQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.DebitOrigin
            .Include(o => o.AppModule)
            .Include(o => o.Debits)
            .OrderBy(o => o.Name)
                .ThenBy(o => o.AppModule.Name)
            .AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        return DataResult<List<DebitOrigin>>.Success(await query.ToListAsync(cancellationToken));
    }
}
