using CQRSDispatch;
using Finance.Application.Legacy.Commands.CurrencyExchangeRates.Owners;
using Finance.Application.Legacy.Commands.Funds.Owners;
using Finance.Application.Legacy.Commands.Incomes.Owners;
using Finance.Application.Legacy.Commands.Users;
using Finance.Application.Legacy.Services;
using Finance.Application.Legacy.Services.Interfaces;
using Finance.Application.Legacy.Services.Orchestrators.CurrencyExchangeRateOrchestrations;
using Finance.Application.Legacy.Services.Orchestrators.FundPermissionsOrchestrations;
using Finance.Application.Legacy.Services.Orchestrators.IncomePermissionsOrchestrations;
using Finance.Application.Legacy.Services.RequestBuilders;
using Finance.Application.Legacy.Services.Requests.CurrencyExchangeRates;
using Finance.Application.Legacy.Services.Requests.Funds;
using Finance.Application.Legacy.Services.Requests.Incomes;
using Finance.Application.Services;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Funds;
using Finance.Domain.Models.Incomes;
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

        RegisterResourcePermissionsOrchestrator<
            SetFundOwnerSagaRequest,
            DataResult<FundPermissions>,
            DeleteFundOwnerSagaRequest,
            CommandResult,
            FundPermissionsOrchestrator>(services);

        RegisterResourcePermissionsOrchestrator<
            SetIncomeOwnerSagaRequest,
            DataResult<IncomePermissions>,
            DeleteIncomeOwnerSagaRequest,
            CommandResult,
            IncomePermissionsOrchestrator>(services);

        RegisterResourcePermissionsOrchestrator<
            SetCurrencyExchangeRateOwnerSagaRequest,
            DataResult<CurrencyExchangeRatePermissions>,
            DeleteCurrencyExchangeRateOwnerSagaRequest,
            CommandResult,
            CurrencyExchangeRateOrchestrator>(services);

        RegisterEntityService<
            Fund,
            FundService,
            CreateFundSagaRequest,
            UpdateFundSagaRequest,
            DeleteFundSagaRequest
            >(services);

        RegisterEntityService<
            User,
            UserService,
            CreateUserSagaRequest,
            UpdateUserSagaRequest,
            DeleteUserSagaRequest
            >(services);

        services.AddScoped<SubscriptionService>();

        RegisterEntityService<
            CurrencyExchangeRate,
            CurrencyExchangeRateService,
            CreateCurrencyExchangeRateSagaRequest,
            UpdateCurrencyExchangeRateSagaRequest,
            DeleteCurrencyExchangeRateSagaRequest
            >(services);

        RegisterEntityService<
            Income,
            IncomeService,
            CreateIncomeSagaRequest,
            UpdateIncomeSagaRequest,
            DeleteIncomeSagaRequest
            >(services);

        RegisterResourcePermissionsSagaService<
            CurrencyExchangeRatePermissions,
            CurrencyExchangeRateOrchestrator,
            SetCurrencyExchangeRateOwnerSagaRequest,
            DataResult<CurrencyExchangeRatePermissions>,
            DeleteCurrencyExchangeRateOwnerSagaRequest,
            CommandResult,
            CurrencyExchangeRateOwnerService>(services);

        RegisterResourcePermissionsSagaService<
            FundPermissions,
            FundPermissionsOrchestrator,
            SetFundOwnerSagaRequest,
            DataResult<FundPermissions>,
            DeleteFundOwnerSagaRequest,
            CommandResult,
            FundOwnerService>(services);

        RegisterResourcePermissionsSagaService<
            IncomePermissions,
            IncomePermissionsOrchestrator,
            SetIncomeOwnerSagaRequest,
            DataResult<IncomePermissions>,
            DeleteIncomeOwnerSagaRequest,
            CommandResult,
            IncomeOwnerService>(services);

        RegisterScopedService<IdentityService>(services);

        return services;
    }

    private static void RegisterResourcePermissionsSagaService<TPermissions, TOrchestrator, TSetRequest, TDataResult, TDeleteRequest, TCommandResult, TService>(IServiceCollection services)
        where TPermissions : IEntity
        where TOrchestrator : class, IResourcePermissionsOrchestrator<TSetRequest, TDataResult, TDeleteRequest, TCommandResult>, new()
        where TSetRequest : ISagaRequest
        where TDataResult : RequestResult, new()
        where TDeleteRequest : ISagaRequest
        where TCommandResult : RequestResult, new()
        where TService : class, IResourcePermissionsSagaService<TPermissions, TOrchestrator, TSetRequest, TDataResult, TDeleteRequest, TCommandResult>
    {
        RegisterScopedService<TService, IResourcePermissionsSagaService<TPermissions, TOrchestrator, TSetRequest, TDataResult, TDeleteRequest, TCommandResult>>(services);
    }

    private static void RegisterResourcePermissionsOrchestrator<TSetRequest, TDataResult, TDeleteRequest, TCommandResult, TOrchestrator>(IServiceCollection services)
        where TSetRequest : ISagaRequest
        where TDataResult : RequestResult, new()
        where TDeleteRequest : ISagaRequest
        where TCommandResult : RequestResult, new()
        where TOrchestrator : class, IResourcePermissionsOrchestrator<TSetRequest, TDataResult, TDeleteRequest, TCommandResult>
    {
        RegisterScopedService<TOrchestrator, IResourcePermissionsOrchestrator<TSetRequest, TDataResult, TDeleteRequest, TCommandResult>>(services);
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
