using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Core.Config;

public class DatabaseSeeder : IHostedService
{
    private readonly IServiceProvider provider;

    public DatabaseSeeder(IServiceProvider serviceProvider)
    {
        provider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();

        bool saveChanges = false;
        if (!dbContext.Currency.Any())
        {
            await dbContext.Currency.AddRangeAsync(
                new Currency { Name = CurrencyNames.Peso, ShortName = CurrencyNames.Peso, },
                new Currency { Name = CurrencyNames.Dollar, ShortName = CurrencyNames.Dollar, });
            saveChanges = true;
        }

        if (saveChanges) await dbContext.SaveChangesAsync();

        if (!dbContext.Module.Any())
        {
            var currencyPeso = await dbContext.Currency.FirstOrDefaultAsync(x => x.Name == CurrencyNames.Peso);
            if (currencyPeso == null) throw new SystemException("Fatal error while seeding App database");

            await dbContext.Module.AddRangeAsync(new List<Module>
            {
                new Module { Name = ModuleNames.Funds, CreatedAt = DateTime.Now.ToUniversalTime(), Currency = currencyPeso },
            });
            saveChanges = true;
        }

        if (saveChanges) await dbContext.SaveChangesAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static class CurrencyNames
    {
        public const string Peso = "Peso";
        public const string Dollar = "Dollar";
    }

    private static class ModuleNames
    {
        public const string Funds = "Fondos";
    }
}
