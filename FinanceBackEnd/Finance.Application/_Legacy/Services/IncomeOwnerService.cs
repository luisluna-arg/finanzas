using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Legacy.Commands.Incomes.Owners;
using Finance.Application.Legacy.Services.Base;
using Finance.Application.Legacy.Services.Orchestrators.IncomePermissionsOrchestrations;
using Finance.Domain.Models.Auth;
using Finance.Persistence;

namespace Finance.Application.Legacy.Services;

public class IncomeOwnerService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext)
    : BaseResourcePermissionsSagaService<
        IncomePermissions,
        IncomePermissionsOrchestrator,
        SetIncomeOwnerSagaRequest,
        DataResult<IncomePermissions>,
        DeleteIncomeOwnerSagaRequest,
        CommandResult>(dispatcher, dbContext);
