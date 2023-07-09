using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Core.Config;

public static class ConfigExtensions
{
    public static void MainServices(this IServiceCollection services)
    {
        services.AddMediatR(o => o.RegisterServicesFromAssembly(typeof(Program).Assembly));

        services.AddScoped<IRepository<Currency, Guid>, CurrencyRepository>();
        services.AddScoped<IRepository<CurrencyConversion, Guid>, CurrencyConversionRepository>();
        services.AddScoped<IRepository<Movement, Guid>, MovementRepository>();
        services.AddScoped<IAppModuleRepository, AppModuleRepository>();
    }
}
