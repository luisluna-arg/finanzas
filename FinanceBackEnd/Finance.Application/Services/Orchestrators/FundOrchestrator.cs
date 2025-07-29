using CQRSDispatch;
using Finance.Application.Commands.Funds;
using Finance.Application.Commands.Funds.Owners;
using Finance.Application.Services.Base;
using Finance.Application.Services.Orchestrators;
using Finance.Application.Services.Requests.Funds;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;

namespace Finance.Application.Services;

public class FundOrchestrator
    : BaseResourceOrchestrator<
        Fund,
    FundResource,
    CreateFundSagaRequest,
    UpdateFundSagaRequest,
    DeleteFundSagaRequest,
    SetFundOwnerSagaRequest,
    DataResult<FundResource>,
    DeleteFundOwnerSagaRequest,
    CommandResult,
    FundResourceOrchestrator>
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

        var fundResourceOwnerResult = await OwnerService.Set(
            new SetFundOwnerSagaRequest(createFundResult.Data.Id),
            transaction: transaction,
            httpRequest: httpRequest);

        if (!fundResourceOwnerResult.IsSuccess)
        {
            throw new Exception(fundResourceOwnerResult.ErrorMessage);
        }

        return createFundResult;
    }

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

    public override async Task<CommandResult> OrchestrateDelete(DeleteFundSagaRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var command = new DeleteFundsCommand
        {
            Ids = [request.Id]
        };
        var result = await Dispatcher.DispatchAsync(command);

        if (!result.IsSuccess)
        {
            throw new Exception(result.ErrorMessage);
        }

        return result;
    }
}