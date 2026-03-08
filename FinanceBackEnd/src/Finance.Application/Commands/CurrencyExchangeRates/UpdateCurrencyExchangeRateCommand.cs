using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Commands.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;

namespace Finance.Application.Commands.CurrencyExchangeRates;

public class UpdateCurrencyExchangeRateCommand : ICommand<DataResult<CurrencyExchangeRate>>
{
    public Guid Id { get; set; }
    public decimal BuyRate { get; set; } = 0m;
    public decimal SellRate { get; set; } = 0m;
}

public class UpdateCurrencyExchangeRateCommandHandler(
    FinanceDbContext db,
    IRepository<CurrencyExchangeRate, Guid> currencyExchangeRateRepository)
    : BaseCommandHandler<UpdateCurrencyExchangeRateCommand, CurrencyExchangeRate>(db)
{
    public override async Task<DataResult<CurrencyExchangeRate>> ExecuteAsync(
        UpdateCurrencyExchangeRateCommand command, CancellationToken cancellationToken)
    {
        var rate = await currencyExchangeRateRepository.GetByIdAsync(command.Id, cancellationToken);
        if (rate == null) throw new Exception("Currency exchange rate not found");

        rate.BuyRate = command.BuyRate;
        rate.SellRate = command.SellRate;

        await currencyExchangeRateRepository.UpdateAsync(rate, cancellationToken);

        return DataResult<CurrencyExchangeRate>.Success(rate);
    }
}
