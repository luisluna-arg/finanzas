using Microsoft.Extensions.DependencyInjection;
using Finance.Application.Mapping.Mappers;

namespace Finance.Application.Mapping;

public static class MapperRegistration
{
    public static IServiceCollection AddMappers(this IServiceCollection services)
    {
        // Register mapping service
        services.AddScoped<IMappingService, MappingService>();
        
        // Register all mappers
        services.AddScoped<IAppModuleMapper, AppModuleMapper>();
        services.AddScoped<IAppModuleTypeMapper, AppModuleTypeMapper>();
        services.AddScoped<IBankMapper, BankMapper>();
        services.AddScoped<ICurrencyMapper, CurrencyMapper>();
        services.AddScoped<IDebitMapper, DebitMapper>();
        services.AddScoped<IFundMapper, FundMapper>();
        services.AddScoped<IFrequencyMapper, FrequencyMapper>();
        services.AddScoped<IIncomeMapper, IncomeMapper>();
        services.AddScoped<IMovementMapper, MovementMapper>();
        
        // Add more mappers as you create them
        
        return services;
    }
}
