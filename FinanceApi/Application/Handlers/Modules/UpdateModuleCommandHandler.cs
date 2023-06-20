using FinanceApi.Application.Commands.Modules;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Modules;

public class UpdateModuleCommandHandler : IRequestHandler<UpdateModuleCommand, Module>
{
    private readonly FinanceDbContext dbContext;

    public UpdateModuleCommandHandler(FinanceDbContext db)
    {
        dbContext = db;
    }

    public async Task<Module> Handle(UpdateModuleCommand command, CancellationToken cancellationToken)
    {
        var module = await GetModule(command.Id);
        var currency = await GetCurrency(command.CurrencyId);

        module.Currency = currency;
        module.Name = command.Name;

        await dbContext.SaveChangesAsync();

        return await Task.FromResult(module);
    }

    private async Task<Module> GetModule(Guid id)
    {
        var module = await dbContext.Module.FirstOrDefaultAsync(o => o.Id == id);
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
