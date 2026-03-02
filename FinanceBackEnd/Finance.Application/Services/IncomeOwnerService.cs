using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.Incomes.Owners;
using Finance.Application.Services.Base;
using Finance.Application.Services.Orchestrators.IncomePermissionsOrchestrations;
using Finance.Domain.Models.Auth;
using Finance.Persistence;

namespace Finance.Application.Services;

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
