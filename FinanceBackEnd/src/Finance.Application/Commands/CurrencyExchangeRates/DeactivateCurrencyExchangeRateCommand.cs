using Finance.Application.Commands.Base;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;

namespace Finance.Application.Commands.CurrencyExchangeRates;

public class DeactivateCurrencyExchangeRateCommand : BatchUpdateBaseCommand;

public class DeactivateCurrencyExchangeRateCommandHandler(FinanceDbContext dbContext)
    : BaseDeactivateCommandHandler<DeactivateCurrencyExchangeRateCommand, DeactivateCurrencyExchangeRateCommandValidator, CurrencyExchangeRate, Guid>(dbContext);

public class DeactivateCurrencyExchangeRateCommandValidator : BatchUpdateBaseCommandValidator<DeactivateCurrencyExchangeRateCommand>;
