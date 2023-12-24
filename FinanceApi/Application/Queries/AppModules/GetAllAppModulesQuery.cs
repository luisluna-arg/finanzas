using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.AppModules;

public class GetAllAppModulesQueryHandler : BaseCollectionHandler<GetAllAppModulesQuery, AppModule?>
{
    public GetAllAppModulesQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<AppModule?>> Handle(GetAllAppModulesQuery request, CancellationToken cancellationToken)
    {
        var query = DbContext.AppModule
            .Include(o => o.Currency)
            .Include(o => o.Type)
            .AsQueryable();

        if (!request.IncludeDeactivated)
        {
            query = query.Where(o => !o.Deactivated);
        }

        return await Task.FromResult(await query.ToArrayAsync());
    }
}

public class GetAllAppModulesQuery : GetAllQuery<AppModule?>
{
}
