using Finance.Domain.Models;
using Finance.Application.Repositories.Base;
using Finance.Persistence;
using Finance.Persistence.Constants;
using Microsoft.EntityFrameworkCore;

namespace Finance.Application.Repositories;

public class AppModuleRepository(FinanceDbContext dbContext) : BaseRepository<AppModule, Guid>(dbContext), IAppModuleRepository
{
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

    async Task<AppModule> IAppModuleRepository.GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var appModule = await DbContext
            .AppModule
            .Include(o => o.Currency)
            .Include(o => o.Type)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        if (appModule == null) throw new Exception($"App Module with id {id} not found");

        return appModule;
    }
}
