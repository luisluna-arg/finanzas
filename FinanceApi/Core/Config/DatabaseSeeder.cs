using FinanceApi.Domain;
using FinanceApi.Domain.Enums;
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

        await SeedDefaultCurrencies(dbContext);

        await SeedAppModules(dbContext);

        await SeedInvestmentAssetIOLTypes(dbContext);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task SeedDefaultCurrencies(FinanceDbContext dbContext)
    {
        var currencies = await dbContext.Currency.ToArrayAsync();

        var newCurrencies = new List<Currency>();

        Action<string, string> collectCurrency = (currencyName, currencyShortName) =>
        {
            if (!currencies.Any(o => o.Name.Equals(currencyName, StringComparison.InvariantCulture)))
            {
                newCurrencies.Add(new Currency { Name = currencyName, ShortName = currencyShortName });
            }
        };

        collectCurrency(CurrencyNames.Peso, CurrencyNames.Peso);
        collectCurrency(CurrencyNames.Dollar, CurrencyNames.Dollar);

        if (newCurrencies.Any())
        {
            await dbContext.AddRangeAsync(newCurrencies);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task SeedAppModules(FinanceDbContext dbContext)
    {
        var currencies = await dbContext.Currency.ToArrayAsync();
        var appModules = await dbContext.AppModule.ToArrayAsync();

        var now = DateTime.Now.ToUniversalTime();

        var newModules = new List<AppModule>();

        Action<string, string> collectModule = (moduleName, currencyName) =>
        {
            if (!appModules.Any(o => o.Name.Equals(moduleName, StringComparison.InvariantCulture)))
            {
                var currencyPeso = currencies.FirstOrDefault(x => x.Name == currencyName);
                if (currencyPeso == null) throw new SystemException($"Fatal error while seeding App database: Currency not found {currencyName}");

                newModules.Add(new AppModule { Name = moduleName, CreatedAt = now, Currency = currencyPeso });
            }
        };

        collectModule(AppModuleNames.Funds, CurrencyNames.Peso);

        collectModule(AppModuleNames.DollarFunds, CurrencyNames.Dollar);

        collectModule(AppModuleNames.IOLInvestments, CurrencyNames.Peso);

        if (newModules.Any())
        {
            await dbContext.AddRangeAsync(newModules);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task SeedInvestmentAssetIOLTypes(FinanceDbContext dbContext)
    {
        var investmentAssetTypes = await dbContext.IOLInvestmentAssetTypes.ToArrayAsync();

        var enumValueInstances = EnumHelper.GetEnumMembers<IOLInvestmentAssetTypeEnum>().ToList();

        var newInvestmentAssetTypes = new List<IOLInvestmentAssetType>();

        Action<ushort, string> collectInvestmentAssetTypes = (id, name) =>
        {
            if (!investmentAssetTypes.Any(o => o.Id == id))
            {
                newInvestmentAssetTypes.Add(new IOLInvestmentAssetType() { Id = id, Name = name });
            }
        };

        enumValueInstances.ForEach(o => collectInvestmentAssetTypes((ushort)o, o.ToString()));

        if (newInvestmentAssetTypes.Any())
        {
            await dbContext.AddRangeAsync(newInvestmentAssetTypes);
            await dbContext.SaveChangesAsync();
        }
    }

    public static class CurrencyNames
    {
        public const string Peso = "Peso";
        public const string Dollar = "Dollar";
    }

    public static class AppModuleNames
    {
        public const string Funds = "Fondos";
        public const string DollarFunds = "Fondos dólares";
        public const string IOLInvestments = "Inversiones IOL";
    }
}
