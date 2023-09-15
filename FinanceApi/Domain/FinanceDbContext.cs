using FinanceApi.Domain.Configurations;
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
    public DbSet<AppModule> AppModule => Set<AppModule>();
    public DbSet<DebitOrigin> DebitOrigin => Set<DebitOrigin>();
    public DbSet<Debit> Debit => Set<Debit>();
    public DbSet<Movement> Movement => Set<Movement>();
    public DbSet<IOLInvestment> IOLInvestments => Set<IOLInvestment>();
    public DbSet<IOLInvestmentAsset> IOLInvestmentAssets => Set<IOLInvestmentAsset>();
    public DbSet<IOLInvestmentAssetType> IOLInvestmentAssetTypes => Set<IOLInvestmentAssetType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseSerialColumns();

        modelBuilder.ApplyConfiguration(new CurrencyConfiguration());
        modelBuilder.ApplyConfiguration(new IOLInvestmentConfiguration());
        modelBuilder.ApplyConfiguration(new IOLInvestmentAssetTypeConfiguration());
        modelBuilder.ApplyConfiguration(new AppModuleConfiguration());
        modelBuilder.ApplyConfiguration(new MovementConfiguration());
        modelBuilder.ApplyConfiguration(new DebitConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
