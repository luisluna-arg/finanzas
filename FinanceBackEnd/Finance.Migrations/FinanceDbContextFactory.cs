using Finance.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Finance.Migrations;

public class FinanceDbContextFactory : IDesignTimeDbContextFactory<FinanceDbContext>
{
    // UserSecretsId defined directly in the Migrations project
    private const string UserSecretsId = "2439153f-d8fa-426d-821a-e701e182b22c";

    public FinanceDbContext CreateDbContext(string[] args)
    {
        // Build configuration within the Migrations project itself
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .AddUserSecrets(UserSecretsId)
            .AddEnvironmentVariables();

        var configuration = configurationBuilder.Build();

        // Get the connection string from configuration
        var connectionString = configuration.GetConnectionString("PostgresDb");

        // Create DbContext options
        var optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();
        optionsBuilder.UseNpgsql(connectionString, o => o.MigrationsAssembly(GetType().Assembly));

        return new FinanceDbContext(optionsBuilder.Options);
    }
}
