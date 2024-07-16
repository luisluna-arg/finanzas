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

    public async Task<AppModule> GetFundsAsync(CancellationToken cancellationToken)
    {
        var appModule = await GetModuleAsync(AppModuleConstants.FundsId, cancellationToken);

        if (appModule == null) throw new Exception("Funds App Module not found");

        return appModule;
    }

    public async Task<AppModule> GetDollarFundsAsync(CancellationToken cancellationToken)
    {
        var appModule = await GetModuleAsync(AppModuleConstants.DollarFundsId, cancellationToken);

        if (appModule == null) throw new Exception("Dollar Funds App Module not found");

        return appModule;
    }

    private async Task<AppModule?> GetModuleAsync(string appModuleId, CancellationToken cancellationToken)
        => await GetAllBy("Id", new Guid(appModuleId)).Include(o => o.Currency).FirstOrDefaultAsync(cancellationToken);
}
