using FinanceApi.Application.Commands.AppModules;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.AppModules;

public class CreateAppModuleCommandHandler : IRequestHandler<CreateAppModuleCommand, AppModule>
{
    private readonly FinanceDbContext dbContext;

    public CreateAppModuleCommandHandler(FinanceDbContext db)
    {
        dbContext = db;
    }

    public async Task<AppModule> Handle(CreateAppModuleCommand request, CancellationToken cancellationToken)
    {
        var currency = await GetCurrency(request.CurrencyId);

        var newAppModule = new AppModule()
        {
            CreatedAt = DateTime.UtcNow,
            Currency = currency,
            Name = request.Name
        };

        dbContext.AppModule.Add(newAppModule);
        await dbContext.SaveChangesAsync();

        return await Task.FromResult(newAppModule);
    }

    private async Task<Currency> GetCurrency(Guid currencyId)
    {
        var currency = await dbContext.Currency.FirstOrDefaultAsync(o => o.Id == currencyId);
        if (currency == null) throw new Exception("Fund currency not found");
        return currency;
    }
}
