using CQRSDispatch;
using Finance.Application.Commands.CurrencyExchangeRates.Owners;
using Finance.Application.Commands.Funds.Owners;
using Finance.Application.Commands.Users;
using Finance.Application.Services;
using Finance.Application.Services.Interfaces;
using Finance.Application.Services.Orchestrators;
using Finance.Application.Services.RequestBuilders;
using Finance.Application.Services.Requests.CurrencyExchangeRates;
using Finance.Application.Services.Requests.Funds;
using Finance.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.Application.Extensions;

public static class SagaServiceExtensions
{
    public static void AddSagaServices(this IServiceCollection services)
    {
        // TODO Copy RepositoryExtensions 
        services.AddScoped<FundResourceOrchestrator>();
        services.AddScoped<IResourceOwnerOrchestrator<
            SetFundOwnerSagaRequest,
            DataResult<FundResource>,
            DeleteFundOwnerSagaRequest,
            CommandResult>, FundResourceOrchestrator>();

        services.AddScoped<CurrencyExchangeRateResourceOrchestrator>();
        services.AddScoped<IResourceOwnerOrchestrator<
            SetCurrencyExchangeRateOwnerSagaRequest,
            DataResult<CurrencyExchangeRateResource>,
            DeleteCurrencyExchangeRateOwnerSagaRequest,
            CommandResult>, CurrencyExchangeRateResourceOrchestrator>();

        services.AddScoped<CurrencyConversionService>();

        services.AddScoped<CurrencyExchangeRateOwnerService>();
        services.AddScoped<IResourceOwnerSagaService<
            CurrencyExchangeRateResource,
            CurrencyExchangeRateResourceOrchestrator,
            SetCurrencyExchangeRateOwnerSagaRequest,
            DataResult<CurrencyExchangeRateResource>,
            DeleteCurrencyExchangeRateOwnerSagaRequest,
            CommandResult>, CurrencyExchangeRateOwnerService>();

        services.AddScoped<CurrencyExchangeRateService>();
        services.AddScoped<ISagaService<
            CreateCurrencyExchangeRateSagaRequest,
            UpdateCurrencyExchangeRateSagaRequest,
            DeleteCurrencyExchangeRateSagaRequest,
            CurrencyExchangeRate>, CurrencyExchangeRateService>();

        services.AddScoped<FundOwnerService>();
        services.AddScoped<IResourceOwnerSagaService<
            FundResource,
            FundResourceOrchestrator,
            SetFundOwnerSagaRequest,
            DataResult<FundResource>,
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