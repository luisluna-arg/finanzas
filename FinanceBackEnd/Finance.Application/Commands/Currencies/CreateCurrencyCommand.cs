using System.ComponentModel.DataAnnotations;
using Finance.Application.Base.Handlers;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Commands.Currencies;

public class CreateCurrencyCommandHandler : BaseCommandHandler<CreateCurrencyCommand, Currency>
{
    private readonly IRepository<Currency, Guid> currencyRepository;

    public CreateCurrencyCommandHandler(
        FinanceDbContext db,
        IRepository<Currency, Guid> currencyRepository)
        : base(db)
    {
        this.currencyRepository = currencyRepository;
    }

    public override async Task<DataResult<Currency>> ExecuteAsync(CreateCurrencyCommand command, CancellationToken cancellationToken)
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

        return DataResult<Currency>.Success(newCurrency);
    }
}

public class CreateCurrencyCommand : ICommand
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string ShortName { get; set; } = string.Empty;
    [Required]
    public ICollection<string> CurrencySymbols { get; set; } = [];
}

