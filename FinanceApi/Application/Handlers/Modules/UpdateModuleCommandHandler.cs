using FinanceApi.Application.Commands.AppModules;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Modules;

public class UpdateModuleCommandHandler : IRequestHandler<UpdateAppModuleCommand, AppModule>
{
    private readonly FinanceDbContext dbContext;

    public UpdateModuleCommandHandler(FinanceDbContext db)
    {
        dbContext = db;
    }

    public async Task<AppModule> Handle(UpdateAppModuleCommand command, CancellationToken cancellationToken)
    {
        var module = await GetModule(command.Id);
        var currency = await GetCurrency(command.CurrencyId);

        module.Currency = currency;
        module.Name = command.Name;

        await dbContext.SaveChangesAsync();

        return await Task.FromResult(module);
    }

    private async Task<AppModule> GetModule(Guid id)
    {
        var module = await dbContext.AppModule.FirstOrDefaultAsync(o => o.Id == id);
        if (module == null) throw new Exception("Module not found");
        return module;
    }

    private async Task<Currency> GetCurrency(Guid id)
    {
        var currency = await dbContext.Currency.FirstOrDefaultAsync(o => o.Id == id);
        if (currency == null) throw new Exception("Currency not found");
        return currency;
    }
}
