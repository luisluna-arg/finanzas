using CQRSDispatch.Interfaces;
using Finance.Persistance;
using Finance.Domain.Models;
using Finance.Application.Auth;
using Finance.Application.Services.Orchestrators;
using CQRSDispatch;
using Finance.Application.Services.Base;
using Finance.Application.Commands.Funds.Owners;

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
        CommandResult>(dispatcher, dbContext)
{
}