using FinanceApi.Models;
using Microsoft.EntityFrameworkCore;

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
    }
}
