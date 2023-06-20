using FinanceApi.Core.EntityFramework.Converters;
using FinanceApi.Core.SpecialTypes;
using FinanceApi.Domain.Enums;
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
    public DbSet<Module> Module => Set<Module>();
    public DbSet<ModuleEntry> ModuleEntry => Set<ModuleEntry>();
    public DbSet<Movement> Movement => Set<Movement>();
    public DbSet<InvestmentAssetIOL> InvestmentAssetIOLs => Set<InvestmentAssetIOL>();
    public DbSet<InvestmentAssetIOLType> InvestmentAssetIOLTypes => Set<InvestmentAssetIOLType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseSerialColumns();

        modelBuilder.Entity<Module>()
            .HasMany(c => c.Movements)
            .WithOne(e => e.Module)
            .IsRequired();

        modelBuilder
            .Entity<Movement>()
            .Property(o => o.TimeStamp)
            .HasConversion(o => o.ToUniversalTime(), o => o);

        modelBuilder
            .Entity<Currency>(entity =>
            {
                entity.HasIndex(o => o.Name).IsUnique();
                entity.HasIndex(o => o.ShortName).IsUnique();
            });

        modelBuilder
            .Entity<InvestmentAssetIOL>()
            .Property(p => p.AssetType)
            .HasConversion<short>();

        modelBuilder
            .Entity<InvestmentAssetIOLType>()
            .HasData(Enum.GetValues(typeof(InvestmentAssetIOLTypeEnum))
                .Cast<InvestmentAssetIOLTypeEnum>()
                .Select(e => new InvestmentAssetIOLType
                {
                    Id = (short)e,
                    Name = e.ToString()
                }));

        this.SetTypeConverters(modelBuilder);
    }

    private void SetTypeConverters(ModelBuilder modelBuilder)
    {
        var moneyConverter = new MoneyValueConverter();

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(Money))
                {
                    property.SetValueConverter(moneyConverter);
                }
            }
        }
    }
}
