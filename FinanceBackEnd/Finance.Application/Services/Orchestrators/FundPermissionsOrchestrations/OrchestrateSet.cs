using CQRSDispatch;
using Finance.Application.Commands;
using Finance.Application.Commands.Funds.Owners;
using Finance.Application.Queries.Resources;
using Finance.Domain.Models.Auth;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Services.Orchestrators.FundPermissionsOrchestrations;

public sealed partial class FundPermissionsOrchestrator : BaseResourcePermissionsOrchestrator<SetFundOwnerSagaRequest, DataResult<FundPermissions>, DeleteFundOwnerSagaRequest, CommandResult>
{
    public override async Task<DataResult<FundPermissions>> OrchestrateSet(SetFundOwnerSagaRequest request, HttpRequest? httpRequest)
    {
        var ResourcePermissions = await Dispatcher.DispatchQueryAsync(new GetFundOwnershipQuery(request.Id), httpRequest);
        if (ResourcePermissions.IsSuccess && ResourcePermissions.Data?.Any() == true)
        {
            return DataResult<FundPermissions>.Success(ResourcePermissions.Data.First().ResourcePermissions);
        }

        var createFundPermissionsCommand = new CreateFundPermissionsCommand
        {
            ResourceId = request.Id,
            UserId = request.UserId!.Value,
            PermissionLevels = [PermissionLevelEnum.Owner]
        };
        var FundPermissionsResult = await Dispatcher.DispatchAsync(createFundPermissionsCommand);
        if (!FundPermissionsResult.IsSuccess || FundPermissionsResult.Data == null)
        {
            throw new Exception(FundPermissionsResult.ErrorMessage);
        }

        return DataResult<FundPermissions>.Success(FundPermissionsResult.Data);
    }
}
