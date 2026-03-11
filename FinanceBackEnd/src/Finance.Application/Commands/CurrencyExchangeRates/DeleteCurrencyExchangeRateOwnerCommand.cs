using Finance.Application.Commands.Base;
using Finance.Domain.Models.Auth;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;

namespace Finance.Application.Commands.CurrencyExchangeRates;

public sealed class DeleteCurrencyExchangeRateOwnerCommand
    : DeleteEntityOwnerCommand<CurrencyExchangeRate, Guid, CurrencyExchangeRatePermissions>;

public sealed class DeleteCurrencyExchangeRateOwnerCommandHandler(FinanceDbContext dbContext)
    : DeleteEntityOwnerCommandHandler<DeleteCurrencyExchangeRateOwnerCommand, CurrencyExchangeRate, Guid, CurrencyExchangeRatePermissions>(dbContext);
