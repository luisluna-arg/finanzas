using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.Funds.Owners;
using Finance.Application.Services.Base;
using Finance.Application.Services.Orchestrators;
using Finance.Domain.Models.Auth;
using Finance.Persistence;

namespace Finance.Application.Services;

public class FundOwnerService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext)
    : BaseResourceOwnerSagaService<
        FundResource,
        FundResourceOrchestrator,
        SetFundOwnerSagaRequest,
        DataResult<FundResource>,
        DeleteFundOwnerSagaRequest,
        CommandResult>(dispatcher, dbContext);
