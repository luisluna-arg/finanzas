using Finance.Application.Commands.Base;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;

namespace Finance.Application.Commands.CurrencyExchangeRates;

public class ActivateCurrencyExchangeRateCommand : BatchUpdateBaseCommand;

public class ActivateCurrencyExchangeRateCommandHandler(FinanceDbContext dbContext)
    : BaseActivateCommandHandler<ActivateCurrencyExchangeRateCommand, ActivateCurrencyExchangeRateCommandValidator, CurrencyExchangeRate, Guid>(dbContext);

public class ActivateCurrencyExchangeRateCommandValidator : BatchUpdateBaseCommandValidator<ActivateCurrencyExchangeRateCommand>;
