using FinanceApi.Application.Commands.AppModules;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.AppModules;

public class CreateAppModuleCommandHandler : BaseResponseHandler<CreateAppModuleCommand, AppModule>
{
    public CreateAppModuleCommandHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<AppModule> Handle(CreateAppModuleCommand command, CancellationToken cancellationToken)
    {
        var currency = await GetCurrency(command.CurrencyId);

        var newAppModule = new AppModule()
        {
            CreatedAt = DateTime.UtcNow,
            Currency = currency,
            Name = command.Name
        };

        DbContext.AppModule.Add(newAppModule);
        await DbContext.SaveChangesAsync();

        return await Task.FromResult(newAppModule);
    }

    private async Task<Currency> GetCurrency(Guid currencyId)
    {
        var currency = await DbContext.Currency.FirstOrDefaultAsync(o => o.Id == currencyId);
        if (currency == null) throw new Exception("Fund currency not found");
        return currency;
    }
}
