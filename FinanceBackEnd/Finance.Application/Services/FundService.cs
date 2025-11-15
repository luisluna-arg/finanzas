using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.Funds.Owners;
using Finance.Application.Services.Base;
using Finance.Application.Services.Interfaces;
using Finance.Application.Services.Orchestrators;
using Finance.Application.Services.Requests.Funds;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Funds;
using Finance.Persistence;

namespace Finance.Application.Services;

public class FundService
    : BaseResourceSagaService<
        Fund,
        FundResource,
        FundOrchestrator,
        FundResourceOrchestrator,
        CreateFundSagaRequest,
        UpdateFundSagaRequest,
        DeleteFundSagaRequest,
        SetFundOwnerSagaRequest,
        DataResult<FundResource>,
        DeleteFundOwnerSagaRequest,
        CommandResult>
{
    public FundService(
        IDispatcher<FinanceDispatchContext> dispatcher,
        FinanceDbContext dbContext,
        IResourceOwnerSagaService<
            FundResource,
            FundResourceOrchestrator,
            SetFundOwnerSagaRequest,
            DataResult<FundResource>,
            DeleteFundOwnerSagaRequest,
            CommandResult> ownerService)
        : base(dispatcher, dbContext, ownerService)
    {
    }
}
