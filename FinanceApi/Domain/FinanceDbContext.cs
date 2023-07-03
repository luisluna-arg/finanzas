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
    public DbSet<Movement> Movement => Set<Movement>();
    public DbSet<InvestmentAssetIOL> InvestmentAssetIOLs => Set<InvestmentAssetIOL>();
    public DbSet<InvestmentAssetIOLType> InvestmentAssetIOLTypes => Set<InvestmentAssetIOLType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseSerialColumns();

        modelBuilder.ApplyConfiguration(new CurrencyConfiguration());
        modelBuilder.ApplyConfiguration(new InvestmentAssetIOLTypeConfiguration());
        modelBuilder.ApplyConfiguration(new AppModuleConfiguration());
        modelBuilder.ApplyConfiguration(new MovementConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
