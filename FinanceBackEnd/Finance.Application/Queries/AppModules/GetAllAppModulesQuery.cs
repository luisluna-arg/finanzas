using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.AppModules;

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

        if (request.AppModuleType.HasValue)
        {
            query = query.Where(o => o.Type.Id == (short)request.AppModuleType.Value);
        }

        return await Task.FromResult(await query.ToArrayAsync());
    }
}

public class GetAllAppModulesQuery : GetAllQuery<AppModule?>
{
    public AppModuleTypeEnum? AppModuleType { get; set; }
}
