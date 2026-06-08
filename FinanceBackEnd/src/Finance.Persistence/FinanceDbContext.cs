using Finance.Domain.Models.AppModules;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Banks;
using Finance.Domain.Models.CreditCards;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Debits;
using Finance.Domain.Models.Frequencies;
using Finance.Domain.Models.Funds;
using Finance.Domain.Models.Identities;
using Finance.Domain.Models.Incomes;
using Finance.Domain.Models.Interfaces;
using Finance.Domain.Models.IOLInvestments;
using Finance.Domain.Models.Movements;
using Finance.Domain.Models.Subscriptions;
using Finance.Domain.SpecialTypes;
using Finance.Persistence.Configurations;
using Finance.Persistence.Extensions;
using Finance.Persistence.TypeConverters;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Finance.Persistence;

public class FinanceDbContext : DbContext
{
    public DbSet<AppModule> AppModule => Set<AppModule>();
    public DbSet<AppModuleType> AppModuleType => Set<AppModuleType>();
    public DbSet<Bank> Bank => Set<Bank>();
    public DbSet<CreditCardIssuer> CreditCardIssuer => Set<CreditCardIssuer>();
    public DbSet<CreditCard> CreditCard => Set<CreditCard>();
    public DbSet<CreditCardStatement> CreditCardStatement => Set<CreditCardStatement>();
    public DbSet<CreditCardStatementImportTemplate> CreditCardStatementImportTemplate => Set<CreditCardStatementImportTemplate>();
    public DbSet<CreditCardStatementTransaction> CreditCardStatementTransaction => Set<CreditCardStatementTransaction>();
    public DbSet<CreditCardTransaction> CreditCardTransaction => Set<CreditCardTransaction>();
    public DbSet<CreditCardPayment> CreditCardPayment => Set<CreditCardPayment>();
    public DbSet<CreditCardStatementAdjustment> CreditCardStatementAdjustment => Set<CreditCardStatementAdjustment>();
    public DbSet<Currency> Currency => Set<Currency>();
    public DbSet<CurrencyConversion> CurrencyConversion => Set<CurrencyConversion>();
    public DbSet<CurrencyExchangeRate> CurrencyExchangeRate => Set<CurrencyExchangeRate>();
    public DbSet<CurrencySymbol> CurrencySymbols => Set<CurrencySymbol>();
    public DbSet<Debit> Debit => Set<Debit>();
    public DbSet<DebitOrigin> DebitOrigin => Set<DebitOrigin>();
    public DbSet<Frequency> Frequency => Set<Frequency>();
    public DbSet<Fund> Fund => Set<Fund>();
    public DbSet<Identity> Identity => Set<Identity>();
    public DbSet<IdentityProvider> IdentityProvider => Set<IdentityProvider>();
    public DbSet<Income> Income => Set<Income>();
    public DbSet<IOLInvestment> IOLInvestment => Set<IOLInvestment>();
    public DbSet<IOLInvestmentAsset> IOLInvestmentAsset => Set<IOLInvestmentAsset>();
    public DbSet<IOLInvestmentAssetType> IOLInvestmentAssetType => Set<IOLInvestmentAssetType>();
    public DbSet<Movement> Movement => Set<Movement>();
    public DbSet<User> User => Set<User>();
    public DbSet<Role> Role => Set<Role>();
    public DbSet<CreditCardPermissions> CreditCardPermissions => Set<CreditCardPermissions>();
    public DbSet<CurrencyExchangeRatePermissions> CurrencyExchangeRatePermissions => Set<CurrencyExchangeRatePermissions>();
    public DbSet<DebitOriginPermissions> DebitOriginPermissions => Set<DebitOriginPermissions>();
    public DbSet<DebitPermissions> DebitPermissions => Set<DebitPermissions>();
    public DbSet<FundPermissions> FundPermissions => Set<FundPermissions>();
    public DbSet<IncomePermissions> IncomePermissions => Set<IncomePermissions>();
    public DbSet<IOLInvestmentAssetPermissions> IOLInvestmentAssetPermissions => Set<IOLInvestmentAssetPermissions>();
    public DbSet<IOLInvestmentPermissions> IOLInvestmentPermissions => Set<IOLInvestmentPermissions>();
    public DbSet<MovementPermissions> MovementPermissions => Set<MovementPermissions>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    private IHttpContextAccessor? HttpContextAccessor { get; }

    internal string CurrentUserId => HttpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "IdentityNotFound";

    public FinanceDbContext(DbContextOptions<FinanceDbContext> options, IHttpContextAccessor? httpContextAccessor)
        : base(options)
    {
        HttpContextAccessor = httpContextAccessor;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseSerialColumns();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppModuleConfiguration).Assembly);

        modelBuilder.AddQueryFilters(this);

        base.OnModelCreating(modelBuilder);
    }

    private static TPermission BuildOwnership<TResource, TId, TPermission>(TId id, Guid userId)
        where TResource : IEntity
        where TPermission : ResourcePermissions<TResource, TId>, new()
        => new TPermission { ResourceId = id, UserId = userId, PermissionLevels = [PermissionLevelEnum.Owner] };

    private static readonly Dictionary<Type, Func<Guid, Guid, object>> _permissionFactories = new()
    {
        [typeof(CreditCard)] = BuildOwnership<CreditCard, Guid, CreditCardPermissions>,
        [typeof(CurrencyExchangeRate)] = BuildOwnership<CurrencyExchangeRate, Guid, CurrencyExchangeRatePermissions>,
        [typeof(Debit)] = BuildOwnership<Debit, Guid, DebitPermissions>,
        [typeof(DebitOrigin)] = BuildOwnership<DebitOrigin, Guid, DebitOriginPermissions>,
        [typeof(Fund)] = BuildOwnership<Fund, Guid, FundPermissions>,
        [typeof(Income)] = BuildOwnership<Income, Guid, IncomePermissions>,
        [typeof(IOLInvestment)] = BuildOwnership<IOLInvestment, Guid, IOLInvestmentPermissions>,
        [typeof(IOLInvestmentAsset)] = BuildOwnership<IOLInvestmentAsset, Guid, IOLInvestmentAssetPermissions>,
        [typeof(Movement)] = BuildOwnership<Movement, Guid, MovementPermissions>,
        [typeof(Subscription)] = BuildOwnership<Subscription, Guid, SubscriptionPermissions>,
    };

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await AutoCreateOwnershipPermissionsAsync(cancellationToken);
        SetAuditableDefaults();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        AutoCreateOwnershipPermissions();
        SetAuditableDefaults();
        return base.SaveChanges();
    }

    private async Task AutoCreateOwnershipPermissionsAsync(CancellationToken cancellationToken)
    {
        if (HttpContextAccessor?.HttpContext == null) return;

        var addedOwned = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added && _permissionFactories.ContainsKey(e.Entity.GetType()))
            .Select(e => (Type: e.Entity.GetType(), Id: (Guid)e.Property("Id").CurrentValue!))
            .ToList();

        if (addedOwned.Count == 0) return;

        var user = await User
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(u => u.Identities)
            .FirstOrDefaultAsync(u => u.Identities.Any(i => i.SourceId == CurrentUserId), cancellationToken);

        if (user == null) return;

        foreach (var (type, entityId) in addedOwned)
            Add(_permissionFactories[type](entityId, user.Id));
    }

    private void AutoCreateOwnershipPermissions()
    {
        if (HttpContextAccessor?.HttpContext == null) return;

        var addedOwned = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added && _permissionFactories.ContainsKey(e.Entity.GetType()))
            .Select(e => (Type: e.Entity.GetType(), Id: (Guid)e.Property("Id").CurrentValue!))
            .ToList();

        if (addedOwned.Count == 0) return;

        var user = User
            .IgnoreQueryFilters()
            .Include(u => u.Identities)
            .FirstOrDefault(u => u.Identities.Any(i => i.SourceId == CurrentUserId));

        if (user == null) return;

        foreach (var (type, entityId) in addedOwned)
            Add(_permissionFactories[type](entityId, user.Id));
    }

    private void SetAuditableDefaults()
    {
        foreach (var entry in ChangeTracker.Entries<IAuditedEntity>())
        {
            if (entry.State == EntityState.Added && entry.Entity.CreatedAt == default)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<Money>()
            .HaveConversion<MoneyValueConverter>();

        configurationBuilder
            .Properties<Money?>()
            .HaveConversion<NullableMoneyValueConverter>();

        configurationBuilder
            .Properties<DateTime>()
            .HaveConversion<DateTimeUtcConverter>();

        configurationBuilder
            .Properties<DateTime?>()
            .HaveConversion<NullableDateTimeUtcConverter>();
    }
}
