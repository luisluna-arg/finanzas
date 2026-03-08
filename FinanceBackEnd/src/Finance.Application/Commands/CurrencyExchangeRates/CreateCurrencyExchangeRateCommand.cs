using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Auth;
using Finance.Application.Commands.Base;
using Finance.Application.Repositories;
using Finance.Domain.Models.Currencies;
using Finance.Persistence;
using FinanceBackEnd.Finance.Domain.Enums;

namespace Finance.Application.Commands.CurrencyExchangeRates;

public class CreateCurrencyExchangeRateCommand : IContextAwareCommand<FinanceDispatchContext, DataResult<CurrencyExchangeRate>>
{
    public Guid BaseCurrencyId { get; set; }
    public Guid QuoteCurrencyId { get; set; }
    public decimal BuyRate { get; set; } = 0m;
    public decimal SellRate { get; set; } = 0m;
    public DateTime TimeStamp { get; set; }
    internal FinanceDispatchContext Context { get; private set; } = new();
    public void SetContext(FinanceDispatchContext context) => Context = context;
}

public class CreateCurrencyExchangeRateCommandHandler(
    FinanceDbContext db,
    IRepository<Currency, Guid> currencyRepository,
    IRepository<CurrencyExchangeRate, Guid> currencyExchangeRateRepository,
    IDispatcher<FinanceDispatchContext> dispatcher)
    : BaseCommandHandler<CreateCurrencyExchangeRateCommand, CurrencyExchangeRate>(db)
{
    public override async Task<DataResult<CurrencyExchangeRate>> ExecuteAsync(
        CreateCurrencyExchangeRateCommand command, CancellationToken cancellationToken)
    {
        var baseCurrency = await currencyRepository.GetByIdAsync(command.BaseCurrencyId, cancellationToken);
        if (baseCurrency == null) throw new Exception("Base currency not found");

        var quoteCurrency = await currencyRepository.GetByIdAsync(command.QuoteCurrencyId, cancellationToken);
        if (quoteCurrency == null) throw new Exception("Quote currency not found");

        var newRate = new CurrencyExchangeRate()
        {
            BaseCurrency = baseCurrency,
            QuoteCurrency = quoteCurrency,
            BuyRate = command.BuyRate,
            SellRate = command.SellRate,
            TimeStamp = command.TimeStamp.Ticks != 0 ? command.TimeStamp : DateTime.Now
        };

        await currencyExchangeRateRepository.AddAsync(newRate, cancellationToken);

        await dispatcher.DispatchAsync(
            new CreateCurrencyExchangeRatePermissionsCommand
            {
                ResourceId = newRate.Id,
                PermissionLevels = [PermissionLevelEnum.Owner],
            },
            command.Context.HttpRequest);

        return DataResult<CurrencyExchangeRate>.Success(newRate);
    }
}
