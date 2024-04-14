using FinanceApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Domain;

public class FinanceDbContext : DbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options)
        : base(options)
    {
    }

    public DbSet<Bank> Bank => Set<Bank>();
    public DbSet<Currency> Currency => Set<Currency>();
    public DbSet<CurrencyConversion> CurrencyConversion => Set<CurrencyConversion>();
    public DbSet<CurrencyExchangeRate> CurrencyExchangeRate => Set<CurrencyExchangeRate>();
    public DbSet<AppModule> AppModule => Set<AppModule>();
    public DbSet<AppModuleType> AppModuleType => Set<AppModuleType>();
    public DbSet<Debit> Debit => Set<Debit>();
    public DbSet<DebitOrigin> DebitOrigin => Set<DebitOrigin>();
    public DbSet<Movement> Movement => Set<Movement>();
    public DbSet<IOLInvestment> IOLInvestment => Set<IOLInvestment>();
    public DbSet<IOLInvestmentAsset> IOLInvestmentAsset => Set<IOLInvestmentAsset>();
    public DbSet<IOLInvestmentAssetType> IOLInvestmentAssetType => Set<IOLInvestmentAssetType>();
    public DbSet<CreditCardMovement> CreditCardMovement => Set<CreditCardMovement>();
    public DbSet<CreditCard> CreditCard => Set<CreditCard>();
    public DbSet<CreditCardStatement> CreditCardStatement => Set<CreditCardStatement>();
    public DbSet<Fund> Fund => Set<Fund>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseSerialColumns();

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Program).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
