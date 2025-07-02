using Finance.Domain.Models;
using Finance.Persistance.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Finance.Persistance;

public class FinanceDbContext(DbContextOptions<FinanceDbContext> options) : DbContext(options)
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
    public DbSet<UserRole> UserRole => Set<UserRole>();
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
    public DbSet<IOLInvestmentAssetTypeResource> IOLInvestmentAssetTypeResource => Set<IOLInvestmentAssetTypeResource>();
    public DbSet<IOLInvestmentResource> IOLInvestmentResource => Set<IOLInvestmentResource>();
    public DbSet<MovementResource> MovementResource => Set<MovementResource>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseSerialColumns();

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppModuleConfiguration).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
