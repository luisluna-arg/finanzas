using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Queries.Base;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Queries.AppModules;

public class GetAllAppModulesQueryHandler : BaseCollectionHandler<GetAllAppModulesQuery, AppModule>
{
    public GetAllAppModulesQueryHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<DataResult<List<AppModule>>> ExecuteAsync(GetAllAppModulesQuery request, CancellationToken cancellationToken)
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
            query = query.Where(o => o.Type.Id == request.AppModuleType.Value);
        }

        // Ensure all required properties are loaded
        var result = await query.ToListAsync(cancellationToken);

        // Validate all modules have their required properties
        foreach (var module in result)
        {
            if (module.Currency == null || module.Type == null)
            {
                Console.WriteLine($"Warning: AppModule {module.Id} has null Currency or Type property.");
            }
        }

        return DataResult<List<AppModule>>.Success(result);
    }
}

public class GetAllAppModulesQuery : GetAllQuery<AppModule>
{
    public AppModuleTypeEnum? AppModuleType { get; set; }
}
