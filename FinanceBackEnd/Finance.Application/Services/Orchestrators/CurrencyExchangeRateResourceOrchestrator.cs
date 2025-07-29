using CQRSDispatch;
using Finance.Application.Commands;
using Finance.Application.Commands.CurrencyExchangeRates;
using Finance.Application.Commands.CurrencyExchangeRates.Owners;
using Finance.Application.Queries.CurrencyExchangeRates;
using Finance.Application.Queries.Resources;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Services.Orchestrators;

public sealed class CurrencyExchangeRateResourceOrchestrator : BaseResourceOwnerOrchestrator<SetCurrencyExchangeRateOwnerSagaRequest, DataResult<CurrencyExchangeRateResource>, DeleteCurrencyExchangeRateOwnerSagaRequest, CommandResult>
{
    public override async Task<DataResult<CurrencyExchangeRateResource>> OrchestrateSet(SetCurrencyExchangeRateOwnerSagaRequest request, HttpRequest? httpRequest)
    {
        var resourceOwnership = await Dispatcher.DispatchQueryAsync(new GetCurrencyExchangeRateOwnershipQuery(request.Id), httpRequest);
        if (resourceOwnership.IsSuccess && resourceOwnership.Data.Any())
        {
            return DataResult<CurrencyExchangeRateResource>.Success(resourceOwnership.Data.First().EntityResource);
        }

        var query = new GetCurrencyExchangeRateQuery() { Id = request.Id };
        var currencyExchangeRateResult = await Dispatcher.DispatchQueryAsync(query);
        if (!currencyExchangeRateResult.IsSuccess || currencyExchangeRateResult.Data == null)
        {
            throw new Exception(currencyExchangeRateResult.ErrorMessage);
        }

        var createResourceResult = await Dispatcher.DispatchAsync(new CreateResourceCommand());
        if (!createResourceResult.IsSuccess || createResourceResult.Data == null)
        {
            throw new Exception(createResourceResult.ErrorMessage);
        }

        var createCurrencyExchangeRateResourceCommand = new CreateCurrencyExchangeRateResourceCommand
        {
            Id = currencyExchangeRateResult.Data!.Id,
            ResourceId = createResourceResult.Data.Id
        };
        var currencyExchangeRateResourceResult = await Dispatcher.DispatchAsync(createCurrencyExchangeRateResourceCommand);
        if (!currencyExchangeRateResourceResult.IsSuccess || currencyExchangeRateResourceResult.Data == null)
        {
            throw new Exception(currencyExchangeRateResourceResult.ErrorMessage);
        }

        var createResourceOwnerCommand = new CreateResourceOwnerCommand
        {
            ResourceId = createResourceResult.Data.Id
        };
        var resourceOwnerResult = await Dispatcher.DispatchAsync(createResourceOwnerCommand, httpRequest);
        if (!resourceOwnerResult.IsSuccess || resourceOwnerResult.Data == null)
        {
            throw new Exception(resourceOwnerResult.ErrorMessage);
        }

        return DataResult<CurrencyExchangeRateResource>.Success(currencyExchangeRateResourceResult.Data);
    }

    public override async Task<CommandResult> OrchestrateDelete(DeleteCurrencyExchangeRateOwnerSagaRequest request, HttpRequest? httpRequest)
    {
        var deleteCurrencyExchangeRateOwnerCommand = new DeleteCurrencyExchangeRateCommand
        {
            Ids = request.Ids
        };
        var createResourceResult = await Dispatcher.DispatchAsync(deleteCurrencyExchangeRateOwnerCommand);

        if (!createResourceResult.IsSuccess)
        {
            throw new Exception(createResourceResult.ErrorMessage);
        }

        return CommandResult.Success();
    }
}