using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories.Base;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Infrastructure.Repositotories;

public class AppModuleRepository : BaseRepository<AppModule, Guid>, IAppModuleRepository
{
    public AppModuleRepository(FinanceDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<AppModule> GetFund()
    {
        var appModule = await DbContext.AppModule.SingleOrDefaultAsync(o => o.Name == "Fondos");

        if (appModule == null) throw new Exception("Fund App Module not found");

        return appModule;
    }
}
