using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;

namespace FinanceApi.Core.Config;

public static class ConfigExtensions
{
    public static void MainServices(this IServiceCollection services)
    {
        services.AddMediatR(o => o.RegisterServicesFromAssembly(typeof(Program).Assembly));

        services.AddScoped<IAppModuleRepository, AppModuleRepository>();
        services.AddScoped<IRepository<Bank, Guid>, BankRepository>();
        services.AddScoped<IRepository<Currency, Guid>, CurrencyRepository>();
        services.AddScoped<IRepository<CurrencyConversion, Guid>, CurrencyConversionRepository>();
        services.AddScoped<IRepository<IOLInvestment, Guid>, IOLInvestmentRepository>();
        services.AddScoped<IRepository<IOLInvestmentAsset, Guid>, IOLInvestmentAssetRepository>();
        services.AddScoped<IRepository<IOLInvestmentAssetType, ushort>, IOLInvestmentAssetTypeRepository>();
        services.AddScoped<IRepository<Movement, Guid>, MovementRepository>();
    }
}
