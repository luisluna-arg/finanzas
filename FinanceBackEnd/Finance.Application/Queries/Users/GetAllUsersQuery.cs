using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Users;

public class GetAllUsersQuery : GetAllQuery<User>;

public class GetAllUsersQueryHandler(FinanceDbContext db) : BaseCollectionHandler<GetAllUsersQuery, User>(db)
{
    public override async Task<DataResult<List<User>>> ExecuteAsync(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.User
            .Include(u => u.Roles)
            .Include(u => u.Identities)
            .AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        return DataResult<List<User>>.Success(await query.ToListAsync(cancellationToken));
    }
}
