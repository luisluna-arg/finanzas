using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Legacy.Commands.Funds.Owners;
using Finance.Application.Legacy.Services.Base;
using Finance.Application.Legacy.Services.Interfaces;
using Finance.Application.Legacy.Services.Orchestrators.FundOrchestrations;
using Finance.Application.Legacy.Services.Orchestrators.FundPermissionsOrchestrations;
using Finance.Application.Legacy.Services.Requests.Funds;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Funds;
using Finance.Persistence;

namespace Finance.Application.Legacy.Services;

public class FundService
    : BaseResourceSagaService<
        Fund,
        FundPermissions,
        FundOrchestrator,
        FundPermissionsOrchestrator,
        CreateFundSagaRequest,
        UpdateFundSagaRequest,
        DeleteFundSagaRequest,
        SetFundOwnerSagaRequest,
        DataResult<FundPermissions>,
        DeleteFundOwnerSagaRequest,
        CommandResult>
{
    public FundService(
        IDispatcher<FinanceDispatchContext> dispatcher,
        FinanceDbContext dbContext,
        IResourcePermissionsSagaService<
            FundPermissions,
            FundPermissionsOrchestrator,
            SetFundOwnerSagaRequest,
            DataResult<FundPermissions>,
            DeleteFundOwnerSagaRequest,
            CommandResult> ownerService)
        : base(dispatcher, dbContext, ownerService)
    {
    }
}
