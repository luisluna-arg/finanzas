using Finance.Domain.Models;
using Finance.Persistance.Configurations;
using Finance.Persistance.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Finance.Persistance;

public class FinanceDbContext : DbContext
{
    public DbSet<AppModule> AppModule => Set<AppModule>();
    public DbSet<AppModuleType> AppModuleType => Set<AppModuleType>();
    public DbSet<Bank> Bank => Set<Bank>();
    public DbSet<CreditCard> CreditCard => Set<CreditCard>();
    public DbSet<CreditCardMovement> CreditCardMovement => Set<CreditCardMovement>();
    public DbSet<CreditCardStatement> CreditCardStatement => Set<CreditCardStatement>();
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
    public DbSet<Resource> Resource => Set<Resource>();
    public DbSet<ResourceOwner> ResourceOwner => Set<ResourceOwner>();
    public DbSet<CreditCardMovementResource> CreditCardMovementResource => Set<CreditCardMovementResource>();
    public DbSet<CreditCardResource> CreditCardResource => Set<CreditCardResource>();
    public DbSet<CreditCardStatementResource> CreditCardStatementResource => Set<CreditCardStatementResource>();
    public DbSet<CurrencyExchangeRateResource> CurrencyExchangeRateResource => Set<CurrencyExchangeRateResource>();
    public DbSet<DebitOriginResource> DebitOriginResource => Set<DebitOriginResource>();
    public DbSet<DebitResource> DebitResource => Set<DebitResource>();
    public DbSet<FundResource> FundResource => Set<FundResource>();
    public DbSet<IncomeResource> IncomeResource => Set<IncomeResource>();
    public DbSet<IOLInvestmentAssetResource> IOLInvestmentAssetResource => Set<IOLInvestmentAssetResource>();
    public DbSet<IOLInvestmentResource> IOLInvestmentResource => Set<IOLInvestmentResource>();
    public DbSet<MovementResource> MovementResource => Set<MovementResource>();

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
}
