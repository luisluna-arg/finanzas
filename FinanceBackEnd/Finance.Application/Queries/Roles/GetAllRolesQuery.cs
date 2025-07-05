using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.Roles;

public class GetAllRolesQuery : GetAllQuery<Role?>;

public class GetAllRolesQueryHandler(FinanceDbContext db) : BaseCollectionHandler<GetAllRolesQuery, Role?>(db)
{
    public override async Task<ICollection<Role?>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.Role.AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        return await Task.FromResult(await query.ToArrayAsync());
    }
}
