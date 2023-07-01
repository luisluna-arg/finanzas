using FinanceApi.Application.Commands.AppModules;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.AppModules;

public class UpdateAppModuleCommandHandler : IRequestHandler<UpdateAppModuleCommand, AppModule>
{
    private readonly FinanceDbContext dbContext;

    public UpdateAppModuleCommandHandler(FinanceDbContext db)
    {
        dbContext = db;
    }

    public async Task<AppModule> Handle(UpdateAppModuleCommand command, CancellationToken cancellationToken)
    {
        var appModule = await GetAppModule(command.Id);
        var currency = await GetCurrency(command.CurrencyId);

        appModule.Currency = currency;
        appModule.Name = command.Name;

        await dbContext.SaveChangesAsync();

        return await Task.FromResult(appModule);
    }

    private async Task<AppModule> GetAppModule(Guid id)
    {
        var appModule = await dbContext.AppModule.FirstOrDefaultAsync(o => o.Id == id);
        if (appModule == null) throw new Exception("App module not found");
        return appModule;
    }

    private async Task<Currency> GetCurrency(Guid id)
    {
        var currency = await dbContext.Currency.FirstOrDefaultAsync(o => o.Id == id);
        if (currency == null) throw new Exception("Currency not found");
        return currency;
    }
}
