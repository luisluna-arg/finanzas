using Finance.Application.Commands.Users;
using Finance.Application.Services;
using Finance.Application.Services.Interfaces;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Finance.Application.Extensions;

public static class SagaServiceExtensions
{
    public static IServiceCollection AddSagaServices(this IServiceCollection services)
    {
        // TODO Copy RepositoryExtensions 
        RegisterScopedService<CurrencyConversionService>(services);

        services.AddScoped<FundService>();

        RegisterEntityService<
            User,
            UserService,
            CreateUserSagaRequest,
            UpdateUserSagaRequest,
            DeleteUserSagaRequest
            >(services);

        services.AddScoped<IncomeService>();
        services.AddScoped<SubscriptionService>();
        services.AddScoped<CurrencyExchangeRateService>();
        services.AddScoped<DebitService>();
        services.AddScoped<DebitOriginService>();
        services.AddScoped<CreditCardService>();
        services.AddScoped<MovementService>();

        RegisterScopedService<IdentityService>(services);

        return services;
    }

    private static void RegisterEntityService<TEntity, TSagaService, TCreateRequest, TUpdateRequest, TDeleteRequest>(IServiceCollection services)
        where TSagaService : class, ISagaService<TCreateRequest, TUpdateRequest, TDeleteRequest, TEntity>
        where TCreateRequest : ISagaRequest
        where TUpdateRequest : ISagaRequest
        where TDeleteRequest : ISagaRequest
        where TEntity : IEntity?
    {
        RegisterScopedService<TSagaService, ISagaService<
            TCreateRequest,
            TUpdateRequest,
            TDeleteRequest,
            TEntity>>(services);
    }

    private static void RegisterScopedService<TService>(IServiceCollection services)
        where TService : class
    {
        services.TryAddScoped<TService>();
    }

    private static void RegisterScopedService<TService, TInterface>(IServiceCollection services)
        where TService : class, TInterface
        where TInterface : class
    {
        // Register only the interface mapping to avoid redundant concrete type registrations
        services.TryAddScoped<TInterface, TService>();
    }
}
