using CQRSDispatch;
using Finance.Application.Commands;
using Finance.Application.Commands.Funds.Owners;
using Finance.Application.Queries.Resources;
using Finance.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Services.Orchestrators;

public sealed class FundResourceOrchestrator : BaseResourceOwnerOrchestrator<SetFundOwnerSagaRequest, DataResult<FundResource>, DeleteFundOwnerSagaRequest, CommandResult>
{
    public override async Task<DataResult<FundResource>> OrchestrateSet(SetFundOwnerSagaRequest request, HttpRequest? httpRequest)
    {
        var resourceOwnership = await Dispatcher.DispatchQueryAsync(new GetFundOwnershipQuery(request.Id), httpRequest);
        if (resourceOwnership.IsSuccess && resourceOwnership.Data.Any())
        {
            return DataResult<FundResource>.Success(resourceOwnership.Data.First().EntityResource);
        }

        var createResourceResult = await Dispatcher.DispatchAsync(new CreateResourceCommand());
        if (!createResourceResult.IsSuccess || createResourceResult.Data == null)
        {
            throw new Exception(createResourceResult.ErrorMessage);
        }

        var createFundResourceCommand = new CreateFundResourceCommand
        {
            ResourceId = createResourceResult.Data.Id,
            FundId = request.Id
        };
        var fundResourceResult = await Dispatcher.DispatchAsync(createFundResourceCommand);
        if (!fundResourceResult.IsSuccess || fundResourceResult.Data == null)
        {
            throw new Exception(fundResourceResult.ErrorMessage);
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

        return DataResult<FundResource>.Success(fundResourceResult.Data);
    }

    public override async Task<CommandResult> OrchestrateDelete(DeleteFundOwnerSagaRequest request, HttpRequest? httpRequest)
    {
        var deleteFundOwnerCommand = new DeleteFundOwnerCommand
        {
            FundId = request.Id
        };
        var createResourceResult = await Dispatcher.DispatchAsync(deleteFundOwnerCommand);

        if (!createResourceResult.IsSuccess)
        {
            throw new Exception(createResourceResult.ErrorMessage);
        }

        return CommandResult.Success();
    }
}
