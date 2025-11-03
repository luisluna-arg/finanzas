using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.CurrencyExchangeRates.Owners;
using Finance.Application.Services.Base;
using Finance.Application.Services.Orchestrators;
using Finance.Domain.Models;
using Finance.Persistence;

namespace Finance.Application.Services;

public class CurrencyExchangeRateOwnerService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext)
    : BaseResourceOwnerSagaService<
        CurrencyExchangeRateResource,
        CurrencyExchangeRateResourceOrchestrator,
        SetCurrencyExchangeRateOwnerSagaRequest,
        DataResult<CurrencyExchangeRateResource>,
        DeleteCurrencyExchangeRateOwnerSagaRequest,
        CommandResult>(dispatcher, dbContext)
{
}
