using Finance.Application.Base.Handlers;
using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;

namespace Finance.Application.Commands.CurrencyConvertions;

public class UpdateCurrencyConversionCommandHandler : BaseCommandHandler<UpdateCurrencyConversionCommand, CurrencyConversion>
{
    private readonly IRepository<Currency, Guid> _currencyRepository;
    private readonly IRepository<CurrencyConversion, Guid> _currencyConversionRepository;
    private readonly IRepository<Movement, Guid> _movementRepository;

    public UpdateCurrencyConversionCommandHandler(
        FinanceDbContext db,
        IRepository<Currency, Guid> currencyRepository,
        IRepository<CurrencyConversion, Guid> currencyConversionRepository,
        IRepository<Movement, Guid> movementRepository)
        : base(db)
    {
        _currencyRepository = currencyRepository;
        _currencyConversionRepository = currencyConversionRepository;
        _movementRepository = movementRepository;
    }

    public override async Task<DataResult<CurrencyConversion>> ExecuteAsync(UpdateCurrencyConversionCommand command, CancellationToken cancellationToken)
    {
        var currencyConversion = await _currencyConversionRepository.GetByIdAsync(command.Id, cancellationToken);
        if (currencyConversion == null) throw new Exception("Currency Conversion not found");

        var movement = await _movementRepository.GetByIdAsync(command.MovementId, cancellationToken);
        if (movement == null) throw new Exception("Movement not found");

        currencyConversion.Movement = movement;

        currencyConversion.Currency = command.CurrencyId.HasValue ? await _currencyRepository.GetByIdAsync(command.CurrencyId.Value, cancellationToken) : null;

        currencyConversion.Amount = command.Amount;

        await _currencyConversionRepository.UpdateAsync(currencyConversion, cancellationToken);

        return DataResult<CurrencyConversion>.Success(currencyConversion);
    }
}

public class UpdateCurrencyConversionCommand : ICommand
{
    required public Guid Id { get; set; }
    required public Guid MovementId { get; set; }
    public Guid? CurrencyId { get; set; }
    public decimal Amount { get; set; }
}
