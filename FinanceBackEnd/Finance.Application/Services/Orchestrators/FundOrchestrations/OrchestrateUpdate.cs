using CQRSDispatch;
using Finance.Application.Commands.Funds;
using Finance.Application.Commands.Funds.Owners;
using Finance.Application.Services.Base;
using Finance.Application.Services.Orchestrators.FundPermissionsOrchestrations;
using Finance.Application.Services.Requests.Funds;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Funds;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;

namespace Finance.Application.Services.Orchestrators.FundOrchestrations;

public partial class FundOrchestrator
    : BaseResourceOrchestrator<
        Fund,
    FundPermissions,
    CreateFundSagaRequest,
    UpdateFundSagaRequest,
    DeleteFundSagaRequest,
    SetFundOwnerSagaRequest,
    DataResult<FundPermissions>,
    DeleteFundOwnerSagaRequest,
    CommandResult,
    FundPermissionsOrchestrator>
{
    public override async Task<DataResult<Fund>> OrchestrateUpdate(UpdateFundSagaRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var command = new UpdateFundCommand
        {
            Id = request.Id,
            BankId = request.BankId,
            CurrencyId = request.CurrencyId,
            TimeStamp = request.TimeStamp,
            Amount = request.Amount,
            DailyUse = request.DailyUse
        };
        var result = await Dispatcher.DispatchAsync(command);

        if (!result.IsSuccess)
        {
            throw new Exception(result.ErrorMessage);
        }

        return result!;
    }
}
