using Finance.Application.Auth;
using Finance.Application.Commands.Users;
using Finance.Application.Specifications.CreditCards;
using Finance.Application.Services;
using Finance.Application.Services.Interfaces;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Finance.Application.Extensions;

public static class SagaServiceExtensions
{
    public static IServiceCollection AddApplicationSecurityServices(this IServiceCollection services)
    {
        services.RegisterScopedService<IsAdminUser>();
        return services;
    }

    public static IServiceCollection AddSpecifications(this IServiceCollection services)
    {
        services.RegisterScopedService<CanSetSystemFlag>();
        return services;
    }

    public static IServiceCollection AddSagaServices(this IServiceCollection services)
    {
        services.RegisterScopedService<CurrencyConversionService>();

        services.RegisterScopedService<FundService>();

        RegisterEntityService<
            User,
            UserService,
            CreateUserSagaRequest,
            UpdateUserSagaRequest,
            DeleteUserSagaRequest
            >(services);

        services.RegisterScopedService<IncomeService>();
        services.RegisterScopedService<SubscriptionService>();
        services.RegisterScopedService<CurrencyExchangeRateService>();
        services.RegisterScopedService<DebitService>();
        services.RegisterScopedService<DebitOriginService>();
        services.RegisterScopedService<CreditCardService>();
        services.RegisterScopedService<MovementService>();
        services.RegisterScopedService<IOLInvestmentService>();
        services.RegisterScopedService<IOLInvestmentAssetService>();
        services.RegisterScopedService<IdentityService>();

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

    private static void RegisterScopedService<TService>(this IServiceCollection services)
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
