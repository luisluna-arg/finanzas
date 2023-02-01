using FinanceApi.Models;
using Microsoft.EntityFrameworkCore;

internal class FinanceDb : DbContext
{
    public FinanceDb(DbContextOptions<FinanceDb> options)
        : base(options)
    {
    }

    public DbSet<Bank> Bank => this.Set<Bank>();
    public DbSet<ModuleEntry> Fund => this.Set<ModuleEntry>();
    public DbSet<Module> Module => this.Set<Module>();
    public DbSet<Movement> Movement => this.Set<Movement>();
    public DbSet<Currency> Currency => this.Set<Currency>();

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
    }
}
