using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Legacy.Commands.CurrencyExchangeRates.Owners;
using Finance.Application.Legacy.Services.Base;
using Finance.Application.Legacy.Services.Orchestrators.CurrencyExchangeRateOrchestrations;
using Finance.Domain.Models.Auth;
using Finance.Persistence;

namespace Finance.Application.Legacy.Services;

public class CurrencyExchangeRateOwnerService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext)
    : BaseResourcePermissionsSagaService<
        CurrencyExchangeRatePermissions,
        CurrencyExchangeRateOrchestrator,
        SetCurrencyExchangeRateOwnerSagaRequest,
        DataResult<CurrencyExchangeRatePermissions>,
        DeleteCurrencyExchangeRateOwnerSagaRequest,
        CommandResult>(dispatcher, dbContext);
