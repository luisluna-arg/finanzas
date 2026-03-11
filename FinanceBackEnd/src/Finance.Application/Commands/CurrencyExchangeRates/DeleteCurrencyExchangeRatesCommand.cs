using Finance.Application.Commands.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.Currencies;

namespace Finance.Application.Commands.CurrencyExchangeRates;

public class DeleteCurrencyExchangeRatesCommand : DeleteEntityCommand<Guid>;

public sealed class DeleteCurrencyExchangeRatesCommandHandler(IRepository<CurrencyExchangeRate, Guid> repository)
    : DeleteEntityCommandHandler<CurrencyExchangeRate, Guid, DeleteCurrencyExchangeRatesCommand, DeleteCurrencyExchangeRatesCommandValidator>(repository);

public sealed class DeleteCurrencyExchangeRatesCommandValidator()
    : DeleteEntityCommandValidator<DeleteCurrencyExchangeRatesCommand, Guid>();
