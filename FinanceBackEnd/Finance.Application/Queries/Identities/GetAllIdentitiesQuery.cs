using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Identities;

public class GetAllIdentitiesQuery : GetAllQuery<Identity>;

public class GetAllIdentitiesQueryHandler(FinanceDbContext db) : BaseCollectionHandler<GetAllIdentitiesQuery, Identity>(db)
{
    public override async Task<DataResult<List<Identity>>> ExecuteAsync(GetAllIdentitiesQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.Identity.AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        return DataResult<List<Identity>>.Success(await query.ToListAsync(cancellationToken));
    }
}
