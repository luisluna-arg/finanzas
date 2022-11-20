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
    }
}
