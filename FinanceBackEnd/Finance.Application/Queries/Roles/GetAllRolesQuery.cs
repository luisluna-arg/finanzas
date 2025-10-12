using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Roles;

public class GetAllRolesQuery : GetAllQuery<Role>;

public class GetAllRolesQueryHandler(FinanceDbContext db) : BaseCollectionHandler<GetAllRolesQuery, Role>(db)
{
    public override async Task<DataResult<List<Role>>> ExecuteAsync(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.Role.AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        return DataResult<List<Role>>.Success(await query.ToListAsync(cancellationToken));
    }
}
