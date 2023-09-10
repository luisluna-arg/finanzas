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
        if (dbContext.Currency.Any()) return;

        await dbContext.Currency.AddRangeAsync(
            new Currency { Name = CurrencyNames.Peso, ShortName = CurrencyNames.Peso, },
            new Currency { Name = CurrencyNames.Dollar, ShortName = CurrencyNames.Dollar, });

        await dbContext.SaveChangesAsync();
    }

    private async Task SeedAppModules(FinanceDbContext dbContext)
    {
        if (dbContext.AppModule.Any()) return;

        var currencyPeso = await dbContext.Currency.FirstOrDefaultAsync(x => x.Name == CurrencyNames.Peso);
        if (currencyPeso == null) throw new SystemException($"Fatal error while seeding App database: Currency {CurrencyNames.Peso}");

        var currencyDollar = await dbContext.Currency.FirstOrDefaultAsync(x => x.Name == CurrencyNames.Dollar);
        if (currencyDollar == null) throw new SystemException($"Fatal error while seeding App database: Currency {CurrencyNames.Dollar}");

        var now = DateTime.Now.ToUniversalTime();

        await dbContext.AppModule.AddRangeAsync(new List<AppModule>
            {
                new AppModule { Name = AppModuleNames.Funds, CreatedAt = now, Currency = currencyPeso },
                new AppModule { Name = AppModuleNames.DollarFunds, CreatedAt = now, Currency = currencyDollar },
                new AppModule { Name = AppModuleNames.IOLInvestments, CreatedAt = now, Currency = currencyPeso },
            });

        await dbContext.SaveChangesAsync();
    }

    private async Task SeedInvestmentAssetIOLTypes(FinanceDbContext dbContext)
    {
        if (dbContext.IOLInvestmentAssetTypes.Any()) return;

        await dbContext.IOLInvestmentAssetTypes.AddRangeAsync(
            EnumHelper.GetEnumMembers<IOLInvestmentAssetTypeEnum>()
            .Select(x => new IOLInvestmentAssetType() { Id = (ushort)x, Name = x.ToString() }));

        await dbContext.SaveChangesAsync();
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
