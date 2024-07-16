using System.ComponentModel.DataAnnotations;
using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;

namespace FinanceApi.Application.Commands.CurrencyExchangeRates;

public class CreateCurrencyExchangeRateCommandHandler : BaseResponseHandler<CreateCurrencyExchangeRateCommand, CurrencyExchangeRate>
{
    private readonly IRepository<Currency, Guid> currencyRepository;
    private readonly IRepository<CurrencyExchangeRate, Guid> currencyExchangeRateRepository;

    public CreateCurrencyExchangeRateCommandHandler(
        FinanceDbContext db,
        IRepository<Currency, Guid> currencyRepository,
        IRepository<CurrencyExchangeRate, Guid> currencyExchangeRateRepository)
        : base(db)
    {
        this.currencyRepository = currencyRepository;
        this.currencyExchangeRateRepository = currencyExchangeRateRepository;
    }

    public override async Task<CurrencyExchangeRate> Handle(CreateCurrencyExchangeRateCommand command, CancellationToken cancellationToken)
    {
        var baseCurrency = await currencyRepository.GetByIdAsync(command.BaseCurrencyId, cancellationToken);
        if (baseCurrency == null) throw new Exception("Base currency not found");

        var quoteCurrency = await currencyRepository.GetByIdAsync(command.QuoteCurrencyId, cancellationToken);
        if (quoteCurrency == null) throw new Exception("Quote currency not found");

        var newCurrency = new CurrencyExchangeRate()
        {
            BaseCurrency = baseCurrency,
            QuoteCurrency = quoteCurrency,
            BuyRate = command.BuyRate,
            SellRate = command.SellRate,
            TimeStamp = command.TimeStamp.Ticks != 0 ? command.TimeStamp : DateTime.Now
        };

        await currencyExchangeRateRepository.AddAsync(newCurrency, cancellationToken);

        return await Task.FromResult(newCurrency);
    }
}

public class CreateCurrencyExchangeRateCommand : IRequest<CurrencyExchangeRate>
{
    [Required]
    public Guid BaseCurrencyId { get; set; }

    [Required]
    public Guid QuoteCurrencyId { get; set; }

    [Required]
    public decimal BuyRate { get; set; } = 0m;

    [Required]
    public decimal SellRate { get; set; } = 0m;

    [Required]
    public DateTime TimeStamp { get; set; }
}

