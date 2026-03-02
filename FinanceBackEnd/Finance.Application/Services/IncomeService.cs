using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.Incomes.Owners;
using Finance.Application.Services.Base;
using Finance.Application.Services.Interfaces;
using Finance.Application.Services.Orchestrators.IncomeOrchestrations;
using Finance.Application.Services.Orchestrators.IncomePermissionsOrchestrations;
using Finance.Application.Services.Requests.Incomes;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Incomes;
using Finance.Persistence;

namespace Finance.Application.Services;

public class IncomeService
    : BaseResourceSagaService<
        Income,
        IncomePermissions,
        IncomeOrchestrator,
        IncomePermissionsOrchestrator,
        CreateIncomeSagaRequest,
        UpdateIncomeSagaRequest,
        DeleteIncomeSagaRequest,
        SetIncomeOwnerSagaRequest,
        DataResult<IncomePermissions>,
        DeleteIncomeOwnerSagaRequest,
        CommandResult>
{
    public IncomeService(
        IDispatcher<FinanceDispatchContext> dispatcher,
        FinanceDbContext dbContext,
        IResourcePermissionsSagaService<
            IncomePermissions,
            IncomePermissionsOrchestrator,
            SetIncomeOwnerSagaRequest,
            DataResult<IncomePermissions>,
            DeleteIncomeOwnerSagaRequest,
            CommandResult> ownerService)
        : base(dispatcher, dbContext, ownerService)
    {
    }
}
