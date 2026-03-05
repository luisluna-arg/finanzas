using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Legacy.Commands.Incomes.Owners;
using Finance.Application.Legacy.Services.Base;
using Finance.Application.Legacy.Services.Interfaces;
using Finance.Application.Legacy.Services.Orchestrators.IncomeOrchestrations;
using Finance.Application.Legacy.Services.Orchestrators.IncomePermissionsOrchestrations;
using Finance.Application.Legacy.Services.Requests.Incomes;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Incomes;
using Finance.Persistence;

namespace Finance.Application.Legacy.Services;

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
