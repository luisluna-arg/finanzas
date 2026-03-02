using CQRSDispatch;
using Finance.Application.Commands.Incomes;
using Finance.Application.Commands.Incomes.Owners;
using Finance.Application.Services.Base;
using Finance.Application.Services.Orchestrators.IncomePermissionsOrchestrations;
using Finance.Application.Services.Requests.Incomes;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Incomes;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;

namespace Finance.Application.Services.Orchestrators.IncomeOrchestrations;

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
    public override async Task<DataResult<Income>> OrchestrateUpdate(UpdateIncomeSagaRequest request, IDbContextTransaction? transaction = null, HttpRequest? httpRequest = null)
    {
        var command = new UpdateIncomeCommand
        {
            Id = request.IncomeId,
            BankId = request.BankId,
            CurrencyId = request.CurrencyId,
            TimeStamp = request.TimeStamp,
            Amount = request.Amount
        };
        var result = await Dispatcher.DispatchAsync(command);

        if (!result.IsSuccess)
        {
            throw new Exception(result.ErrorMessage);
        }

        return result!;
    }
}
