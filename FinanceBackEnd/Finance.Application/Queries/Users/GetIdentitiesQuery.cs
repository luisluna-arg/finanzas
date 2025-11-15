using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models.Identities;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Users;

public class GetIdentitiesQuery : GetAllQuery<Identity>
{
    public Guid UserId { get; set; }
}

public class GetIdentitiesQueryHandler(FinanceDbContext db) : BaseCollectionHandler<GetIdentitiesQuery, Identity>(db)
{
    public override async Task<DataResult<List<Identity>>> ExecuteAsync(GetIdentitiesQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.Identity
            .Where(i => i.UserId == request.UserId)
            .Include(i => i.Provider)
            .AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        return DataResult<List<Identity>>.Success(await query.ToListAsync(cancellationToken));
    }
}
