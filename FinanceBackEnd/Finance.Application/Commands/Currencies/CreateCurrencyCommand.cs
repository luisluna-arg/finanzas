using System.ComponentModel.DataAnnotations;
using Finance.Application.Base.Handlers;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using MediatR;

namespace Finance.Application.Commands.Currencies;

public class CreateCurrencyCommandHandler : BaseResponseHandler<CreateCurrencyCommand, Currency>
{
    private readonly IRepository<Currency, Guid> currencyRepository;

    public CreateCurrencyCommandHandler(
        FinanceDbContext db,
        IRepository<Currency, Guid> currencyRepository)
        : base(db)
    {
        this.currencyRepository = currencyRepository;
    }

    public override async Task<Currency> Handle(CreateCurrencyCommand command, CancellationToken cancellationToken)
    {
        var newCurrency = new Currency()
        {
            ShortName = command.ShortName,
            Name = command.Name,
            Symbols = command.CurrencySymbols.Select(x => new CurrencySymbol()
            {
                Symbol = x
            }).ToList(),
        };

        await currencyRepository.AddAsync(newCurrency, cancellationToken);

        return await Task.FromResult(newCurrency);
    }
}

public class CreateCurrencyCommand : IRequest<Currency>
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string ShortName { get; set; } = string.Empty;
    [Required]
    public ICollection<string> CurrencySymbols { get; set; } = [];
}

