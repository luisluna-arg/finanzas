using CQRSDispatch;
using Finance.Application.Legacy.Commands.Incomes;
using Finance.Application.Legacy.Commands.Incomes.Owners;
using Finance.Application.Legacy.Services.Base;
using Finance.Application.Legacy.Services.Orchestrators.IncomePermissionsOrchestrations;
using Finance.Application.Legacy.Services.Requests.Incomes;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Incomes;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;

namespace Finance.Application.Legacy.Services.Orchestrators.IncomeOrchestrations;

public partial class IncomeOrchestrator
    : BaseResourceOrchestrator<
        Income,
    IncomePermissions,
    CreateIncomeSagaRequest,
    UpdateIncomeSagaRequest,
    DeleteIncomeSagaRequest,
    SetIncomeOwnerSagaRequest,
    DataResult<IncomePermissions>,
    DeleteIncomeOwnerSagaRequest,
    CommandResult,
    IncomePermissionsOrchestrator>
{
    public override async Task<CommandResult> OrchestrateDelete(DeleteIncomeSagaRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var command = new DeleteIncomesCommand
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
