using CQRSDispatch;
using Finance.Application.Commands.CurrencyExchangeRates;
using Finance.Application.Commands.CurrencyExchangeRates.Owners;
using Finance.Domain.Models.Auth;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Services.Orchestrators.CurrencyExchangeRatePermissionsOrchestrations;

public sealed partial class CurrencyExchangeRatePermissionsOrchestrator
    : BaseResourcePermissionsOrchestrator<
        SetCurrencyExchangeRateOwnerSagaRequest,
        DataResult<CurrencyExchangeRatePermissions>,
        DeleteCurrencyExchangeRateOwnerSagaRequest,
        CommandResult>
{
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
