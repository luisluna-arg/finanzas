using CQRSDispatch;
using Finance.Application.Commands.CurrencyExchangeRates.Owners;
using Finance.Application.Commands.Funds.Owners;
using Finance.Application.Commands.Users;
using Finance.Application.Services;
using Finance.Application.Services.Interfaces;
using Finance.Application.Services.Orchestrators.CurrencyExchangeRatePermissionsOrchestrations;
using Finance.Application.Services.Orchestrators.FundPermissionsOrchestrations;
using Finance.Application.Services.RequestBuilders;
using Finance.Application.Services.Requests.CurrencyExchangeRates;
using Finance.Application.Services.Requests.Funds;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Funds;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.Application.Extensions;

public static class SagaServiceExtensions
{
    public static void AddSagaServices(this IServiceCollection services)
    {
        // TODO Copy RepositoryExtensions 
        services.AddScoped<FundPermissionsOrchestrator>();
        services.AddScoped<IResourcePermissionsOrchestrator<
            SetFundOwnerSagaRequest,
            DataResult<FundPermissions>,
            DeleteFundOwnerSagaRequest,
            CommandResult>, FundPermissionsOrchestrator>();

        services.AddScoped<CurrencyExchangeRatePermissionsOrchestrator>();
        services.AddScoped<IResourcePermissionsOrchestrator<
            SetCurrencyExchangeRateOwnerSagaRequest,
            DataResult<CurrencyExchangeRatePermissions>,
            DeleteCurrencyExchangeRateOwnerSagaRequest,
            CommandResult>, CurrencyExchangeRatePermissionsOrchestrator>();

        services.AddScoped<CurrencyConversionService>();

        services.AddScoped<CurrencyExchangeRateOwnerService>();
        services.AddScoped<IResourcePermissionsSagaService<
            CurrencyExchangeRatePermissions,
            CurrencyExchangeRatePermissionsOrchestrator,
            SetCurrencyExchangeRateOwnerSagaRequest,
            DataResult<CurrencyExchangeRatePermissions>,
            DeleteCurrencyExchangeRateOwnerSagaRequest,
            CommandResult>, CurrencyExchangeRateOwnerService>();

        services.AddScoped<CurrencyExchangeRateService>();
        services.AddScoped<ISagaService<
            CreateCurrencyExchangeRateSagaRequest,
            UpdateCurrencyExchangeRateSagaRequest,
            DeleteCurrencyExchangeRateSagaRequest,
            CurrencyExchangeRate>, CurrencyExchangeRateService>();

        services.AddScoped<FundOwnerService>();
        services.AddScoped<IResourcePermissionsSagaService<
            FundPermissions,
            FundPermissionsOrchestrator,
            SetFundOwnerSagaRequest,
            DataResult<FundPermissions>,
            DeleteFundOwnerSagaRequest,
            CommandResult>, FundOwnerService>();

        services.AddScoped<FundService>();
        services.AddScoped<ISagaService<
            CreateFundSagaRequest,
            UpdateFundSagaRequest,
            DeleteFundSagaRequest,
            Fund>, FundService>();
        services.AddScoped<UserService>();

        services.AddScoped<ISagaService<
            CreateUserSagaRequest,
            UpdateUserSagaRequest,
            DeleteUserSagaRequest,
            User>, UserService>();
        services.AddScoped<IdentityService>();
    }
}
