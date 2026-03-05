using CQRSDispatch;
using Finance.Application.Legacy.Commands;
using Finance.Application.Legacy.Commands.Funds.Owners;
using Finance.Application.Legacy.Queries.Resources;
using Finance.Domain.Models.Auth;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Legacy.Services.Orchestrators.FundPermissionsOrchestrations;

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
            UserId = request.UserId,
            PermissionLevels = [PermissionLevelEnum.Owner]
        };
        var fundPermissionsResult = await Dispatcher.DispatchAsync(createFundPermissionsCommand, httpRequest);
        if (!fundPermissionsResult.IsSuccess || fundPermissionsResult.Data == null)
        {
            throw new Exception(fundPermissionsResult.ErrorMessage);
        }

        return DataResult<FundPermissions>.Success(fundPermissionsResult.Data);
    }
}
