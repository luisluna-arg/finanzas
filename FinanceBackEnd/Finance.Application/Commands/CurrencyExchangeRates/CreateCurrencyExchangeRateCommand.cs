using Finance.Application.Base.Handlers;
using CQRSDispatch;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using Finance.Application.Commands.Users;

namespace Finance.Application.Commands.CurrencyExchangeRates;

public class CreateCurrencyExchangeRateCommandHandler : BaseCommandHandler<CreateCurrencyExchangeRateCommand, CurrencyExchangeRate>
{
    private readonly IRepository<Currency, Guid> _currencyRepository;
    private readonly IRepository<CurrencyExchangeRate, Guid> _currencyExchangeRateRepository;

    public CreateCurrencyExchangeRateCommandHandler(
        FinanceDbContext db,
        IRepository<Currency, Guid> currencyRepository,
        IRepository<CurrencyExchangeRate, Guid> currencyExchangeRateRepository)
        : base(db)
    {
        _currencyRepository = currencyRepository;
        _currencyExchangeRateRepository = currencyExchangeRateRepository;
    }

    public override async Task<DataResult<CurrencyExchangeRate>> ExecuteAsync(CreateCurrencyExchangeRateCommand command, CancellationToken cancellationToken)
    {
        var baseCurrency = await _currencyRepository.GetByIdAsync(command.BaseCurrencyId, cancellationToken);
        if (baseCurrency == null) throw new Exception("Base currency not found");

        var quoteCurrency = await _currencyRepository.GetByIdAsync(command.QuoteCurrencyId, cancellationToken);
        if (quoteCurrency == null) throw new Exception("Quote currency not found");

        var newCurrency = new CurrencyExchangeRate()
        {
            BaseCurrency = baseCurrency,
            QuoteCurrency = quoteCurrency,
            BuyRate = command.BuyRate,
            SellRate = command.SellRate,
            TimeStamp = command.TimeStamp.Ticks != 0 ? command.TimeStamp : DateTime.Now
        };

        await _currencyExchangeRateRepository.AddAsync(newCurrency, cancellationToken);

        return DataResult<CurrencyExchangeRate>.Success(newCurrency);
    }
}

public class CreateCurrencyExchangeRateCommand : OwnerBaseCommand<DataResult<CurrencyExchangeRate>>
{
    public Guid BaseCurrencyId { get; set; }
    public Guid QuoteCurrencyId { get; set; }
    public decimal BuyRate { get; set; } = 0m;
    public decimal SellRate { get; set; } = 0m;
    public DateTime TimeStamp { get; set; }
}

