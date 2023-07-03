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
        if (currencyPeso == null) throw new SystemException("Fatal error while seeding App database");

        await dbContext.AppModule.AddRangeAsync(new List<AppModule>
            {
                new AppModule { Name = AppModuleNames.Funds, CreatedAt = DateTime.Now.ToUniversalTime(), Currency = currencyPeso },
            });

        await dbContext.SaveChangesAsync();
    }

    private async Task SeedInvestmentAssetIOLTypes(FinanceDbContext dbContext)
    {
        if (dbContext.InvestmentAssetIOLTypes.Any()) return;

        await dbContext.InvestmentAssetIOLTypes.AddRangeAsync(
            EnumHelper.GetEnumMembers<InvestmentAssetIOLTypeEnum>()
            .Select(x => new InvestmentAssetIOLType() { Id = (ushort)x, Name = x.ToString() }));

        await dbContext.SaveChangesAsync();
    }

    private static class CurrencyNames
    {
        public const string Peso = "Peso";
        public const string Dollar = "Dollar";
    }

    private static class AppModuleNames
    {
        public const string Funds = "Fondos";
    }
}
