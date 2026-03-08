using Finance.Application.Commands.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;

namespace Finance.Application.Commands.CurrencyExchangeRates;

public class DeleteCurrencyExchangeRateOwnerCommand
    : DeleteEntityOwnerCommand<CurrencyExchangeRate, Guid, CurrencyExchangeRatePermissions>;

public class DeleteCurrencyExchangeRateOwnerCommandHandler(FinanceDbContext dbContext)
    : DeleteEntityOwnerCommandHandler<DeleteCurrencyExchangeRateOwnerCommand, CurrencyExchangeRate, Guid, CurrencyExchangeRatePermissions>(dbContext);
