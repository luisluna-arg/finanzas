using System.Linq.Expressions;
using FinanceApi.Application.Commands.Movements;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Movements;

public class CreateMovementCommandHandler : BaseResponseHandler<CreateMovementCommand, Movement>
{
    public CreateMovementCommandHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<Movement> Handle(CreateMovementCommand command, CancellationToken cancellationToken)
    {
        AppModule? appModule = await GetAppModule(command.AppModuleId);

        Currency? currency = await GetCurrency(command.CurrencyId);

        var newMovement = new Movement()
        {
            AppModule = appModule,
            Currency = currency,
            Amount = command.Amount,
            Concept1 = command.Concept1,
            Concept2 = command.Concept2,
            TimeStamp = command.TimeStamp,
            Total = command.Total,
        };

        DbContext.Movement.Add(newMovement);
        await DbContext.SaveChangesAsync();

        return await Task.FromResult(newMovement);
    }

    private async Task<Currency?> GetCurrency(Guid? currencyId)
    {
        if (!currencyId.HasValue) return null;

        var currency = await DbContext.Currency.FirstOrDefaultAsync(o => o.Id == currencyId);
        if (currency == null) throw new Exception("Fund currency not found");
        return currency;
    }

    private async Task<AppModule> GetAppModule(Guid? appModuleId)
    {
        AppModule? appModule = null;
        Expression<Func<AppModule, bool>> filter = !appModuleId.HasValue ? o => o.Name == "Fondos" : o => o.Id == appModuleId.Value;

        appModule = await DbContext.AppModule.FirstOrDefaultAsync(filter);

        if (appModule == null) throw new Exception("Fund app module not found");

        return appModule;
    }
}
