using FinanceApi.Application.Base.Handlers;
using FinanceApi.Application.Queries.Base;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Queries.AppModules;

public class GetAllAppModulesQueryHandler : BaseCollectionHandler<GetAllAppModulesQuery, AppModule>
{
    public GetAllAppModulesQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<ICollection<AppModule>> Handle(GetAllAppModulesQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(await DbContext.AppModule.Include(o => o.Currency).ToArrayAsync());
    }
}

public class GetAllAppModulesQuery : GetAllQuery<AppModule>
{
}
