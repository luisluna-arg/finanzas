using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using FinanceApi.Infrastructure.Services;

namespace FinanceApi.Core.Config;

public static class ConfigExtensions
{
    public static void MainServices(this IServiceCollection services)
    {
        services.AddMediatR(o => o.RegisterServicesFromAssembly(typeof(Program).Assembly));

        services.AddScoped<IAppModuleRepository, AppModuleRepository>();

        services.AddScoped<IRepository<AppModule, Guid>, AppModuleRepository>();
        services.AddScoped<IRepository<AppModuleType, short>, AppModuleTypeRepository>();
        services.AddScoped<IRepository<Bank, Guid>, BankRepository>();
        services.AddScoped<IRepository<Currency, Guid>, CurrencyRepository>();
        services.AddScoped<IRepository<CurrencyConversion, Guid>, CurrencyConversionRepository>();
        services.AddScoped<IRepository<IOLInvestment, Guid>, IOLInvestmentRepository>();
        services.AddScoped<IRepository<IOLInvestmentAsset, Guid>, IOLInvestmentAssetRepository>();
        services.AddScoped<IRepository<IOLInvestmentAssetType, ushort>, IOLInvestmentAssetTypeRepository>();
        services.AddScoped<IRepository<Movement, Guid>, MovementRepository>();
        services.AddScoped<IRepository<Debit, Guid>, DebitRepository>();
        services.AddScoped<IRepository<DebitOrigin, Guid>, DebitOriginRepository>();
        services.AddScoped<IRepository<CreditCard, Guid>, CreditCardRepository>();
        services.AddScoped<IRepository<CreditCardMovement, Guid>, CreditCardMovementRepository>();

        services.AddScoped<IEntityService<AppModule, Guid>, EntityService<AppModule, Guid>>();
        services.AddScoped<IEntityService<Bank, Guid>, EntityService<Bank, Guid>>();
        services.AddScoped<IEntityService<Currency, Guid>, EntityService<Currency, Guid>>();
        services.AddScoped<IEntityService<CurrencyConversion, Guid>, EntityService<CurrencyConversion, Guid>>();
        services.AddScoped<IEntityService<Debit, Guid>, EntityService<Debit, Guid>>();
        services.AddScoped<IEntityService<DebitOrigin, Guid>, EntityService<DebitOrigin, Guid>>();
        services.AddScoped<IEntityService<IOLInvestment, Guid>, EntityService<IOLInvestment, Guid>>();
        services.AddScoped<IEntityService<IOLInvestmentAsset, Guid>, EntityService<IOLInvestmentAsset, Guid>>();
        services.AddScoped<IEntityService<IOLInvestmentAssetType, ushort>, EntityService<IOLInvestmentAssetType, ushort>>();
        services.AddScoped<IEntityService<Movement, Guid>, EntityService<Movement, Guid>>();
        services.AddScoped<IEntityService<CreditCard, Guid>, EntityService<CreditCard, Guid>>();
        services.AddScoped<IEntityService<CreditCardMovement, Guid>, EntityService<CreditCardMovement, Guid>>();
    }
}
