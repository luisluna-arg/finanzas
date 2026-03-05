using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Legacy.Commands.Funds.Owners;
using Finance.Application.Legacy.Services.Base;
using Finance.Application.Legacy.Services.Orchestrators.FundPermissionsOrchestrations;
using Finance.Domain.Models.Auth;
using Finance.Persistence;

namespace Finance.Application.Legacy.Services;

public class FundOwnerService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext)
    : BaseResourcePermissionsSagaService<
        FundPermissions,
        FundPermissionsOrchestrator,
        SetFundOwnerSagaRequest,
        DataResult<FundPermissions>,
        DeleteFundOwnerSagaRequest,
        CommandResult>(dispatcher, dbContext);
