using Finance.SyncTool.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

Console.WriteLine("Mannual execution");

var builder = new ConfigurationBuilder().AddUserSecrets<Program>();

var configuration = builder.Build();

var SupabaseFinanceDbConnStr = configuration.GetConnectionString("SupabaseFinanceDb");
var PostgresDbConnStr = configuration.GetConnectionString("PostgresDb");

Console.WriteLine($"{SupabaseFinanceDbConnStr}");
Console.WriteLine($"{PostgresDbConnStr}");

Console.WriteLine("Mannual execution");

using var sourceContext = new FinanceSyncDbContext(PostgresDbConnStr);
using var targetContext = new FinanceSyncDbContext(SupabaseFinanceDbConnStr);

targetContext.Database.EnsureCreated();
targetContext.Database.Migrate();

var banks = targetContext.Bank.ToList();

foreach (var bank in banks)
{
    Console.WriteLine($"Banco: {bank.Name}");
}
