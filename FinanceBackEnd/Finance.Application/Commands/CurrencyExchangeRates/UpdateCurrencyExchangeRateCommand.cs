using System.ComponentModel.DataAnnotations;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models;
using Finance.Persistence;

namespace Finance.Application.Commands.CurrencyExchangeRates;

public class UpdateCurrencyExchangeRateCommandHandler : BaseCommandHandler<UpdateCurrencyExchangeRateCommand, CurrencyExchangeRate>
{
    private readonly IRepository<CurrencyExchangeRate, Guid> currencyExchangeRateRepository;

    public UpdateCurrencyExchangeRateCommandHandler(
        FinanceDbContext db,
        IRepository<CurrencyExchangeRate, Guid> currencyExchangeRateRepository)
        : base(db)
    {
        this.currencyExchangeRateRepository = currencyExchangeRateRepository;
    }

    public override async Task<DataResult<CurrencyExchangeRate>> ExecuteAsync(UpdateCurrencyExchangeRateCommand command, CancellationToken cancellationToken)
    {
        var currencyExchangeRate = await currencyExchangeRateRepository.GetByIdAsync(command.Id, cancellationToken);
        if (currencyExchangeRate == null) throw new Exception("Currency exchange rate not found");

        currencyExchangeRate.BuyRate = command.BuyRate;
        currencyExchangeRate.SellRate = command.SellRate;

        await currencyExchangeRateRepository.UpdateAsync(currencyExchangeRate, cancellationToken);

        return DataResult<CurrencyExchangeRate>.Success(currencyExchangeRate);
    }
}

public class UpdateCurrencyExchangeRateCommand : ICommand<DataResult<CurrencyExchangeRate>>
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public decimal BuyRate { get; set; } = 0m;

    [Required]
    public decimal SellRate { get; set; } = 0m;
}
