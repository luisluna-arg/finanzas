using FinanceApi.Application.Commands.AppModules;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.AppModules;

public class UpdateAppModuleCommandHandler : BaseResponseHandler<UpdateAppModuleCommand, AppModule>
{
    public UpdateAppModuleCommandHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<AppModule> Handle(UpdateAppModuleCommand command, CancellationToken cancellationToken)
    {
        var appModule = await GetAppModule(command.Id);
        var currency = await GetCurrency(command.CurrencyId);

        appModule.Currency = currency;
        appModule.Name = command.Name;

        await DbContext.SaveChangesAsync();

        return await Task.FromResult(appModule);
    }

    private async Task<AppModule> GetAppModule(Guid id)
    {
        var appModule = await DbContext.AppModule.FirstOrDefaultAsync(o => o.Id == id);
        if (appModule == null) throw new Exception("App module not found");
        return appModule;
    }

    private async Task<Currency> GetCurrency(Guid id)
    {
        var currency = await DbContext.Currency.FirstOrDefaultAsync(o => o.Id == id);
        if (currency == null) throw new Exception("Currency not found");
        return currency;
    }
}
