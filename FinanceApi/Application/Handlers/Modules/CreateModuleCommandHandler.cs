using FinanceApi.Application.Commands.Modules;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Application.Handlers.Modules;

public class CreateModuleCommandHandler : IRequestHandler<CreateModuleCommand, Module>
{
    private readonly FinanceDbContext dbContext;

    public CreateModuleCommandHandler(FinanceDbContext db)
    {
        dbContext = db;
    }

    public async Task<Module> Handle(CreateModuleCommand request, CancellationToken cancellationToken)
    {
        var currency = await GetCurrency(request.CurrencyId);

        var newModule = new Module()
        {
            CreatedAt = DateTime.UtcNow,
            Currency = currency,
            Name = request.Name
        };

        dbContext.Module.Add(newModule);
        await dbContext.SaveChangesAsync();

        return await Task.FromResult(newModule);
    }

    private async Task<Currency> GetCurrency(Guid currencyId)
    {
        var currency = await dbContext.Currency.FirstOrDefaultAsync(o => o.Id == currencyId);
        if (currency == null) throw new Exception("Fund currency not found");
        return currency;
    }
}
