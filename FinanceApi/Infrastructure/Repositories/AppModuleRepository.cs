using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using static FinanceApi.Core.Config.DatabaseSeeder;

namespace FinanceApi.Infrastructure.Repositories;

public class AppModuleRepository : BaseRepository<AppModule, Guid>, IAppModuleRepository
{
    public AppModuleRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<AppModule> GetFunds()
    {
        var appModule = await GetAllBy("Name", AppModuleNames.Funds).Include(o => o.Currency).FirstOrDefaultAsync();

        if (appModule == null) throw new Exception("Funds App Module not found");

        return appModule;
    }

    public async Task<AppModule> GetDollarFunds()
    {
        var appModule = await GetAllBy("Name", AppModuleNames.DollarFunds).Include(o => o.Currency).FirstOrDefaultAsync();

        if (appModule == null) throw new Exception("Dollar Funds App Module not found");

        return appModule;
    }
}
