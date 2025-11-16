using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.CurrencyExchangeRates.Owners;
using Finance.Application.Services.Base;
using Finance.Application.Services.Orchestrators.CurrencyExchangeRatePermissionsOrchestrations;
using Finance.Domain.Models.Auth;
using Finance.Persistence;

namespace Finance.Application.Services;

public class CurrencyExchangeRateOwnerService(
    IDispatcher<FinanceDispatchContext> dispatcher,
    FinanceDbContext dbContext)
    : BaseResourcePermissionsSagaService<
        CurrencyExchangeRatePermissions,
        CurrencyExchangeRatePermissionsOrchestrator,
        SetCurrencyExchangeRateOwnerSagaRequest,
        DataResult<CurrencyExchangeRatePermissions>,
        DeleteCurrencyExchangeRateOwnerSagaRequest,
        CommandResult>(dispatcher, dbContext);
