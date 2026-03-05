using CQRSDispatch;
using Finance.Application.Legacy.Commands.Funds;
using Finance.Application.Legacy.Commands.Funds.Owners;
using Finance.Application.Legacy.Services.Base;
using Finance.Application.Legacy.Services.Orchestrators.FundPermissionsOrchestrations;
using Finance.Application.Legacy.Services.Requests.Funds;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Funds;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;

namespace Finance.Application.Legacy.Services.Orchestrators.FundOrchestrations;

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
            Id = request.FundId,
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
