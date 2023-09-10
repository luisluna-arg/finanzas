using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories.Base;
using static FinanceApi.Core.Config.DatabaseSeeder;

namespace FinanceApi.Infrastructure.Repositories;

public class AppModuleRepository : BaseRepository<AppModule, Guid>, IAppModuleRepository
{
    public AppModuleRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<AppModule> GetFund()
    {
        var appModule = await GetBy("Name", AppModuleNames.Funds);

        if (appModule == null) throw new Exception("Fund App Module not found");

        return appModule;
    }
}
