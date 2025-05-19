using System.ComponentModel.DataAnnotations;
using Finance.Application.Base.Handlers;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using MediatR;

namespace Finance.Application.Commands.Currencies;

public class UpdateCurrencyCommandHandler : BaseResponseHandler<UpdateCurrencyCommand, Currency>
{
    private readonly IRepository<Currency, Guid> _currencyRepository;
    private readonly IRepository<CurrencySymbol, Guid> _currencySymbolRepository;

    public UpdateCurrencyCommandHandler(
        FinanceDbContext db,
        IRepository<Currency, Guid> currencyRepository,
        IRepository<CurrencySymbol, Guid> currencySymbolRepository)
        : base(db)
    {
        this._currencyRepository = currencyRepository;
        this._currencySymbolRepository = currencySymbolRepository;
    }

    public override async Task<Currency> Handle(UpdateCurrencyCommand command, CancellationToken cancellationToken)
    {
        var currency = await _currencyRepository.GetByIdAsync(command.Id, cancellationToken);
        if (currency == null) throw new Exception("Currency not found");

        currency.Name = command.Name;
        currency.ShortName = command.ShortName;

        if (command?.CurrencySymbols?.Count > 0)
        {
            var symbols = _currencySymbolRepository.GetAllBy("CurrencyId", currency.Id);

            var symbolsToRemove = symbols.Where(symbol =>
                    command.CurrencySymbols.All(s => s.Trim().ToLower() != symbol.Symbol.Trim().ToLower()));

            DbContext.CurrencySymbols.RemoveRange(symbolsToRemove);

            var symbolsToKeep = symbols.Where(symbol =>
                command.CurrencySymbols.Any(s => s.Trim().ToLower() == symbol.Symbol.Trim().ToLower()))
                .ToList();

            symbolsToKeep.AddRange(
                command.CurrencySymbols
                    .Where(symbol => symbolsToKeep.All(s => s.Symbol.Trim().ToLower() != symbol.Trim().ToLower()))
                    .Select(s => new CurrencySymbol
                    {
                        Currency = currency,
                        Symbol = s.Trim()
                    }));

            currency.Symbols = symbolsToKeep;
        }

        await _currencyRepository.UpdateAsync(currency, cancellationToken);

        return await Task.FromResult(currency);
    }
}

public class UpdateCurrencyCommand : IRequest<Currency>
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string ShortName { get; set; } = string.Empty;
    [Required]
    public ICollection<string> CurrencySymbols { get; set; } = [];
}
