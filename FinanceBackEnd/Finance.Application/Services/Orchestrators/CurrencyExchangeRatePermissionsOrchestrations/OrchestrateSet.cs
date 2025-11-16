using CQRSDispatch;
using Finance.Application.Commands;
using Finance.Application.Commands.CurrencyExchangeRates.Owners;
using Finance.Application.Queries.CurrencyExchangeRates;
using Finance.Application.Queries.Resources;
using Finance.Domain.Models.Auth;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Services.Orchestrators.CurrencyExchangeRatePermissionsOrchestrations;

public sealed partial class CurrencyExchangeRatePermissionsOrchestrator
    : BaseResourcePermissionsOrchestrator<
        SetCurrencyExchangeRateOwnerSagaRequest,
        DataResult<CurrencyExchangeRatePermissions>,
        DeleteCurrencyExchangeRateOwnerSagaRequest,
        CommandResult>
{
    public override async Task<DataResult<CurrencyExchangeRatePermissions>> OrchestrateSet(SetCurrencyExchangeRateOwnerSagaRequest request, HttpRequest? httpRequest)
    {
        var resourcePermissions = await Dispatcher.DispatchQueryAsync(new GetCurrencyExchangeRateOwnershipQuery(request.Id), httpRequest);
        if (resourcePermissions.IsSuccess && resourcePermissions.Data.Any())
        {
            return DataResult<CurrencyExchangeRatePermissions>.Success(resourcePermissions.Data.First().ResourcePermissions);
        }

        var resourceQuery = new GetCurrencyExchangeRateQuery() { Id = request.Id };
        var currencyExchangeRateResult = await Dispatcher.DispatchQueryAsync(resourceQuery);
        if (!currencyExchangeRateResult.IsSuccess || currencyExchangeRateResult.Data == null)
        {
            throw new Exception(currencyExchangeRateResult.ErrorMessage);
        }

        var createCommand = new CreateCurrencyExchangeRatePermissionsCommand
        {
            ResourceId = request.Id,
            PermissionLevels = [PermissionLevelEnum.Owner]
        };
        var permissionsResult = await Dispatcher.DispatchAsync(createCommand);
        if (!permissionsResult.IsSuccess || permissionsResult.Data == null)
        {
            throw new Exception(permissionsResult.ErrorMessage);
        }

        return DataResult<CurrencyExchangeRatePermissions>.Success(permissionsResult.Data);
    }
}
