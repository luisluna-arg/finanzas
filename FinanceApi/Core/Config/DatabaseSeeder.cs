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

        await SeedAppModuleTypes(dbContext);

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

        Action<string> collectCurrency = (currencyId) =>
        {
            if (!currencies.Any(o => o.Id.ToString().Equals(currencyId, StringComparison.InvariantCulture)))
            {
                var currencyName = CurrencyConstants.Names[currencyId];
                newCurrencies.Add(new Currency { Id = new Guid(currencyId), Name = currencyName, ShortName = currencyName });
            }
        };

        foreach (var currencyId in CurrencyConstants.CurrencyIds)
        {
            collectCurrency(currencyId);
        }

        if (newCurrencies.Any())
        {
            await dbContext.AddRangeAsync(newCurrencies);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task SeedAppModuleTypes(FinanceDbContext dbContext)
    {
        var appModuleTypes = await dbContext.AppModuleType.ToArrayAsync();

        var newAppModuleTypes = new List<AppModuleType>();

        AppModuleTypeEnum[] enumValues = (AppModuleTypeEnum[])Enum.GetValues(typeof(AppModuleTypeEnum));

        Action<AppModuleTypeEnum> collector = (appModuleTypeEnum) =>
        {
            if (!appModuleTypes.Any(o => o.Id == (short)appModuleTypeEnum))
            {
                newAppModuleTypes.Add(new AppModuleType { Id = (short)appModuleTypeEnum, Name = appModuleTypeEnum });
            }
        };

        foreach (var appModuleType in enumValues)
        {
            collector(appModuleType);
        }

        if (newAppModuleTypes.Any())
        {
            await dbContext.AddRangeAsync(newAppModuleTypes);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task SeedAppModules(FinanceDbContext dbContext)
    {
        var currencies = await dbContext.Currency.ToArrayAsync();
        var appModules = await dbContext.AppModule.ToArrayAsync();
        var appModuleTypes = await dbContext.AppModuleType.ToArrayAsync();

        var now = DateTime.Now.ToUniversalTime();

        var newModules = new List<AppModule>();

        Action<string, string> collectModule = (moduleId, currencyId) =>
        {
            if (!appModules.Any(o => o.Id.ToString().Equals(moduleId, StringComparison.InvariantCulture)))
            {
                var currency = currencies.FirstOrDefault(x => x.Id.ToString().Equals(currencyId, StringComparison.InvariantCulture));
                var currencyName = CurrencyConstants.Names[currencyId];
                if (currency == null) throw new SystemException($"Fatal error while seeding App database: Currency not found {currencyName}");

                if (!AppModuleConstants.Types.ContainsKey(moduleId))
                    throw new SystemException($"Fatal error while seeding App database: AppModuleTypeEnum not found for ModuleId {moduleId}");

                var appModuleType = appModuleTypes.FirstOrDefault(x => x.Id == (short)AppModuleConstants.Types[moduleId]);
                if (appModuleType == null)
                    throw new SystemException($"Fatal error while seeding App database: AppModuleType not found {AppModuleConstants.Types[moduleId]}");

                newModules.Add(new AppModule
                {
                    Id = new Guid(moduleId),
                    Name = AppModuleConstants.Names[moduleId],
                    CreatedAt = now,
                    Currency = currency,
                    Type = appModuleType
                });
            }
        };

        foreach (var appModuleIdPair in AppModuleConstants.AppModuleCurrencyPairs)
        {
            collectModule(appModuleIdPair[0], appModuleIdPair[1]);
        }

        if (newModules.Any())
        {
            await dbContext.AddRangeAsync(newModules);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task SeedInvestmentAssetIOLTypes(FinanceDbContext dbContext)
    {
        var investmentAssetTypes = await dbContext.IOLInvestmentAssetType.ToArrayAsync();

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

    public static class CurrencyConstants
    {
        public const string PesoId = "6d189135-7040-45a1-b713-b1aa6cad1720";
        public const string DollarId = "efbf50bc-34d4-43e9-96f9-9f6213ea11b5";

        private static readonly Dictionary<string, string> NamesValue = new Dictionary<string, string>()
        {
            { PesoId, "Peso" },
            { DollarId, "Dollar" },
        };

        public static Dictionary<string, string> Names => NamesValue;

        public static string[] CurrencyIds => new string[] { PesoId, DollarId };
    }

    public static class AppModuleConstants
    {
        public const string FundsId = "f92f45fe-1c9e-4b65-b32b-b033212a7b27";
        public const string DollarFundsId = "93c77ebf-b726-4148-aebe-1e11abc7b47f";
        public const string DebitsId = "4c1ee918-e8f9-4bed-8301-b4126b56cfc0";
        public const string DollarDebitsId = "03cc66c7-921c-4e05-810e-9764cd365c1d";
        public const string IOLInvestmentsId = "65325dbb-13b0-44ff-82ad-5808a26581a4";

        private static readonly Dictionary<string, AppModuleTypeEnum> TypesValue = new Dictionary<string, AppModuleTypeEnum>()
        {
            { FundsId, AppModuleTypeEnum.Funds },
            { DollarFundsId, AppModuleTypeEnum.Funds },
            { DebitsId, AppModuleTypeEnum.Debits },
            { DollarDebitsId, AppModuleTypeEnum.Debits },
            { IOLInvestmentsId, AppModuleTypeEnum.Investments }
        };

        private static readonly Dictionary<string, string> NamesValue = new Dictionary<string, string>()
        {
            { FundsId, "Fondos" },
            { DollarFundsId, "Fondos dólares" },
            { DebitsId, "Débitos" },
            { DollarDebitsId, "Débitos en dólares" },
            { IOLInvestmentsId, "Inversiones IOL" }
        };

        public static Dictionary<string, string> Names => NamesValue;

        public static Dictionary<string, AppModuleTypeEnum> Types => TypesValue;

        public static string[][] AppModuleCurrencyPairs => new string[][]
        {
            new string[] { FundsId, CurrencyConstants.PesoId },
            new string[] { DollarFundsId, CurrencyConstants.DollarId },
            new string[] { DebitsId, CurrencyConstants.PesoId },
            new string[] { DollarDebitsId, CurrencyConstants.DollarId },
            new string[] { IOLInvestmentsId, CurrencyConstants.PesoId },
        };
    }
}
