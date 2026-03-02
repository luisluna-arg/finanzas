using CQRSDispatch;
using Finance.Application.Commands;
using Finance.Application.Commands.Incomes.Owners;
using Finance.Application.Queries.Resources;
using Finance.Domain.Models.Auth;
using FinanceBackEnd.Finance.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Finance.Application.Services.Orchestrators.IncomePermissionsOrchestrations;

public sealed partial class IncomePermissionsOrchestrator : BaseResourcePermissionsOrchestrator<SetIncomeOwnerSagaRequest, DataResult<IncomePermissions>, DeleteIncomeOwnerSagaRequest, CommandResult>
{
    public override async Task<DataResult<IncomePermissions>> OrchestrateSet(SetIncomeOwnerSagaRequest request, HttpRequest? httpRequest)
    {
        var ResourcePermissions = await Dispatcher.DispatchQueryAsync(new GetIncomeOwnershipQuery(request.Id), httpRequest);
        if (ResourcePermissions.IsSuccess && ResourcePermissions.Data?.Any() == true)
        {
            return DataResult<IncomePermissions>.Success(ResourcePermissions.Data.First().ResourcePermissions);
        }

        var createIncomePermissionsCommand = new CreateIncomePermissionsCommand
        {
            ResourceId = request.Id,
            UserId = request.UserId,
            PermissionLevels = [PermissionLevelEnum.Owner]
        };
        var fundPermissionsResult = await Dispatcher.DispatchAsync(createIncomePermissionsCommand, httpRequest);
        if (!fundPermissionsResult.IsSuccess || fundPermissionsResult.Data == null)
        {
            throw new Exception(fundPermissionsResult.ErrorMessage);
        }

        return DataResult<IncomePermissions>.Success(fundPermissionsResult.Data);
    }
}
