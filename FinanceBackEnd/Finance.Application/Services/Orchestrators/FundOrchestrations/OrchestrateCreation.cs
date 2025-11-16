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
    public override async Task<DataResult<Fund>> OrchestrateCreation(CreateFundSagaRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var command = new CreateFundCommand
        {
            BankId = request.BankId,
            CurrencyId = request.CurrencyId,
            TimeStamp = request.TimeStamp,
            Amount = request.Amount,
            DailyUse = request.DailyUse
        };
        var createFundResult = await Dispatcher.DispatchAsync(command);
        if (!createFundResult.IsSuccess)
        {
            throw new Exception(createFundResult.ErrorMessage);
        }

        var FundPermissionsOwnerResult = await OwnerService.Set(
            new SetFundOwnerSagaRequest(createFundResult.Data.Id),
            transaction: transaction,
            httpRequest: httpRequest);

        if (!FundPermissionsOwnerResult.IsSuccess)
        {
            throw new Exception(FundPermissionsOwnerResult.ErrorMessage);
        }

        return createFundResult;
    }
}
