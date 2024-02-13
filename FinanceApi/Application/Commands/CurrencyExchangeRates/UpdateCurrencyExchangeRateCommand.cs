using System.ComponentModel.DataAnnotations;
using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositories;
using MediatR;

namespace FinanceApi.Application.Commands.CurrencyExchangeRates;

public class UpdateCurrencyExchangeRateCommandHandler : BaseResponseHandler<UpdateCurrencyExchangeRateCommand, CurrencyExchangeRate>
{
    private readonly IRepository<CurrencyExchangeRate, Guid> currencyExchangeRateRepository;

    public UpdateCurrencyExchangeRateCommandHandler(
        FinanceDbContext db,
        IRepository<CurrencyExchangeRate, Guid> currencyExchangeRateRepository)
        : base(db)
    {
        this.currencyExchangeRateRepository = currencyExchangeRateRepository;
    }

    public override async Task<CurrencyExchangeRate> Handle(UpdateCurrencyExchangeRateCommand command, CancellationToken cancellationToken)
    {
        var currencyExchangeRate = await currencyExchangeRateRepository.GetById(command.Id);
        if (currencyExchangeRate == null) throw new Exception("Currency exchange rate not found");

        currencyExchangeRate.BuyRate = command.BuyRate;
        currencyExchangeRate.SellRate = command.SellRate;

        await currencyExchangeRateRepository.Update(currencyExchangeRate);

        return await Task.FromResult(currencyExchangeRate);
    }
}

public class UpdateCurrencyExchangeRateCommand : IRequest<CurrencyExchangeRate>
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public decimal BuyRate { get; set; } = 0m;

    [Required]
    public decimal SellRate { get; set; } = 0m;
}
