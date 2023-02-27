using FinanceApi.Models;
using Microsoft.EntityFrameworkCore;

internal class FinanceDb : DbContext
{
    public FinanceDb(DbContextOptions<FinanceDb> options)
        : base(options)
    {
    }

    public DbSet<Bank> Bank => this.Set<Bank>();
    public DbSet<Currency> Currency => this.Set<Currency>();
    public DbSet<CurrencyConversion> CurrencyConversion => this.Set<CurrencyConversion>();
    public DbSet<Module> Module => this.Set<Module>();
    public DbSet<ModuleEntry> ModuleEntry => this.Set<ModuleEntry>();
    public DbSet<Movement> Movement => this.Set<Movement>();

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
