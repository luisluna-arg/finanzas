using FinanceApi.Application.Commands.Movements;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Movements;

public class CreateFundMovementCommandHandler : BaseResponseHandler<CreateFundMovementCommand, Movement>
{
    public CreateFundMovementCommandHandler(FinanceDbContext db)
        : base(db)
    {
    }

    public override async Task<Movement> Handle(CreateFundMovementCommand command, CancellationToken cancellationToken)
    {
        AppModule? appModule = await GetFundAppModule();

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

    private async Task<AppModule> GetFundAppModule()
    {
        var appModule = await DbContext.AppModule.FirstOrDefaultAsync(o => o.Name == "Fondos");

        if (appModule == null) throw new Exception("Fund app module not found");

        return appModule;
    }
}
