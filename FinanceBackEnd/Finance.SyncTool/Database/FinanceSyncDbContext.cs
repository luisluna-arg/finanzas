using Finance.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Finance.SyncTool.Database;

public class FinanceSyncDbContext : FinanceDbContext
{
    private readonly string _connectionString;

    public FinanceSyncDbContext(string connectionString)
        : base(CreateOptions(connectionString))
    {
        _connectionString = connectionString;
    }

    private static DbContextOptions<FinanceDbContext> CreateOptions(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return optionsBuilder.Options;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }
    }
}
