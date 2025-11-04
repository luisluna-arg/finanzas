using Finance.Application.Helpers;
using Finance.Authentication.Options;
using Finance.Authentication.Services;
using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Finance.Api.Core.Config;

public class DatabaseSeeder : IHostedService
{
    private readonly IServiceProvider provider;
    private readonly AdminUserOptions adminUserOptions;
    private readonly ILogger<DatabaseSeeder> logger;

    public DatabaseSeeder(
        IServiceProvider serviceProvider,
        IOptions<AdminUserOptions> adminUserOptions,
        ILogger<DatabaseSeeder> logger)
    {
        provider = serviceProvider;
        this.adminUserOptions = adminUserOptions.Value;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();

        await SeedDefaultCurrencies(dbContext);

        await SeedAppModuleTypes(dbContext);

        await SeedAppModules(dbContext);

        await SeedInvestmentAssetIOLTypes(dbContext);

        await SeedRoles(dbContext);

        await SeedAdminUser(dbContext);
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
            if (!appModuleTypes.Any(o => o.Id == appModuleTypeEnum))
            {
                newAppModuleTypes.Add(new AppModuleType { Id = appModuleTypeEnum, Name = appModuleTypeEnum.ToString() });
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
        var modulesToUpdate = new List<AppModule>();

        Action<string, string> collectModule = (moduleId, currencyId) =>
        {
            var currency = currencies.FirstOrDefault(x => x.Id.ToString().Equals(currencyId, StringComparison.InvariantCulture));
            var currencyName = CurrencyConstants.Names[currencyId];
            if (currency == null)
            {
                throw new SystemException($"Fatal error while seeding App database: Currency not found {currencyName}");
            }

            if (!AppModuleConstants.Types.ContainsKey(moduleId))
            {
                throw new SystemException($"Fatal error while seeding App database: AppModuleTypeEnum not found for ModuleId {moduleId}");
            }

            var appModuleType = appModuleTypes.FirstOrDefault(x => x.Id == AppModuleConstants.Types[moduleId]);
            if (appModuleType == null)
            {
                throw new SystemException($"Fatal error while seeding App database: AppModuleType not found {AppModuleConstants.Types[moduleId]}");
            }

            var appModule = appModules.FirstOrDefault(o => o.Id.ToString().Equals(moduleId, StringComparison.InvariantCulture));

            if (appModule == null)
            {
                newModules.Add(new AppModule
                {
                    Id = new Guid(moduleId),
                    Name = AppModuleConstants.Names[moduleId],
                    CreatedAt = now,
                    Currency = currency,
                    Type = appModuleType,
                });
            }
            else
            {
                appModule.Name = AppModuleConstants.Names[moduleId];
                appModule.Currency = currency;
                appModule.Type = appModuleType;

                modulesToUpdate.Add(appModule);
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

        if (modulesToUpdate.Any())
        {
            dbContext.UpdateRange(modulesToUpdate);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task SeedInvestmentAssetIOLTypes(FinanceDbContext dbContext)
    {
        var investmentAssetTypes = await dbContext.IOLInvestmentAssetType.ToArrayAsync();

        var enumValueInstances = EnumHelper.GetEnumMembers<IOLInvestmentAssetTypeEnum>().ToList();

        var newInvestmentAssetTypes = new List<IOLInvestmentAssetType>();

        Action<IOLInvestmentAssetTypeEnum, string> collectInvestmentAssetTypes = (id, name) =>
        {
            if (!investmentAssetTypes.Any(o => o.Id == id))
            {
                newInvestmentAssetTypes.Add(new IOLInvestmentAssetType() { Id = id, Name = name });
            }
        };

        enumValueInstances.ForEach(o => collectInvestmentAssetTypes(o, o.ToString()));

        if (newInvestmentAssetTypes.Any())
        {
            await dbContext.AddRangeAsync(newInvestmentAssetTypes);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task SeedRoles(FinanceDbContext dbContext)
    {
        var roles = await dbContext.Role.ToArrayAsync();

        var enumValues = Enum.GetValues(typeof(RoleEnum)).Cast<RoleEnum>().ToArray();
        var newRoles = new List<Role>();

        Action<RoleEnum> collectRole = (roleEnum) =>
        {
            if (!roles.Any(o => o.Id == roleEnum))
            {
                newRoles.Add(new Role { Id = roleEnum, Name = roleEnum.ToString(), Deactivated = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
            }
        };

        foreach (var roleEnum in enumValues)
        {
            collectRole(roleEnum);
        }

        if (newRoles.Any())
        {
            await dbContext.AddRangeAsync(newRoles);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task SeedAdminUser(FinanceDbContext dbContext)
    {
        // Check if admin user seeding is enabled
        if (!this.adminUserOptions.EnableSeeding)
        {
            logger.LogInformation("Admin user seeding is disabled");
            return;
        }

        // Only seed admin user if UserId is configured
        if (string.IsNullOrWhiteSpace(this.adminUserOptions.UserId))
        {
            logger.LogInformation("No admin UserId configured, skipping admin user seeding");
            return;
        }

        // Validate user exists in Auth0 (always required for security)
        using var validationScope = provider.CreateScope();
        var auth0ValidationService = validationScope.ServiceProvider.GetRequiredService<IAuth0UserValidationService>();

        var userExistsInAuth0 = await auth0ValidationService.ValidateUserExistsAsync(this.adminUserOptions.UserId);
        if (!userExistsInAuth0)
        {
            logger.LogWarning("Admin user {AdminUserId} does not exist in Auth0. Skipping admin user seeding for security", this.adminUserOptions.UserId);
            return;
        }

        // Get user info from Auth0 for better seeding
        var auth0UserInfo = await auth0ValidationService.GetUserInfoAsync(this.adminUserOptions.UserId);
        if (auth0UserInfo != null)
        {
            logger.LogInformation("Validated admin user {AdminUserId} exists in Auth0. Email: {Email}", this.adminUserOptions.UserId, auth0UserInfo.Email);
        }

        var now = DateTime.UtcNow;
        string firstName = this.adminUserOptions.DefaultFirstName;
        string lastName = this.adminUserOptions.DefaultLastName;
        string username = this.adminUserOptions.DefaultUsername;
        if (auth0UserInfo != null)
        {
            firstName = auth0UserInfo.GivenName ?? firstName;
            lastName = auth0UserInfo.FamilyName ?? lastName;
            username = auth0UserInfo.Name ?? auth0UserInfo.Email ?? username;
        }

        var adminRoleEntity = await dbContext.Role.FirstOrDefaultAsync(r => r.Id == RoleEnum.Admin);

        // 1. Ensure user exists (by username or email)
        var user = await dbContext.User
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Username == username || (auth0UserInfo != null && u.Username == auth0UserInfo.Email));
        if (user == null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                FirstName = firstName,
                LastName = lastName,
                CreatedAt = now,
                UpdatedAt = now
            };
            await dbContext.User.AddAsync(user);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Created admin user {AdminUserId}", this.adminUserOptions.UserId);
        }

        // 2. Ensure identity exists and is linked to user
        var identity = await dbContext.Identity
            .Include(i => i.User)
            .FirstOrDefaultAsync(i => i.Provider == IdentityProviderEnum.Auth && i.SourceId == this.adminUserOptions.UserId);
        if (identity == null)
        {
            identity = new Identity
            {
                Id = Guid.NewGuid(),
                Provider = IdentityProviderEnum.Auth,
                SourceId = this.adminUserOptions.UserId,
                User = user,
                CreatedAt = now,
                UpdatedAt = now
            };
            await dbContext.Identity.AddAsync(identity);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Created identity for admin user {AdminUserId}", this.adminUserOptions.UserId);
        }
        else if (identity.User == null || identity.User.Id != user.Id)
        {
            // Fix orphaned or mismatched identity
            identity.User = user;
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Re-linked identity to correct user for admin {AdminUserId}", this.adminUserOptions.UserId);
        }

        // 3. Ensure admin role is assigned
        if (adminRoleEntity != null && !user.Roles.Any(r => r.Id == RoleEnum.Admin))
        {
            user.Roles.Add(adminRoleEntity);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Admin role assigned to user {AdminUserId}", this.adminUserOptions.UserId);
        }
        else if (adminRoleEntity != null)
        {
            logger.LogInformation("Admin user {AdminUserId} already has admin role", this.adminUserOptions.UserId);
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
