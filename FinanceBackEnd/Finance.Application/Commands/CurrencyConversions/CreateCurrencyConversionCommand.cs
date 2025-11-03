using CQRSDispatch;
using CQRSDispatch.Interfaces;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models;
using Finance.Domain.SpecialTypes;
using Finance.Persistence;

namespace Finance.Application.Commands.CurrencyConvertions;

public class CreateCurrencyConversionCommandHandler : BaseCommandHandler<CreateCurrencyConversionCommand, CurrencyConversion>
{
    private readonly IRepository<Movement, Guid> _movementRepository;
    private readonly IRepository<Currency, Guid> _currencyRepository;
    private readonly IRepository<CurrencyConversion, Guid> _currencyConversionRepository;

    public CreateCurrencyConversionCommandHandler(
        FinanceDbContext db,
        IRepository<Movement, Guid> movementRepository,
        IRepository<Currency, Guid> currencyRepository,
        IRepository<CurrencyConversion, Guid> currencyConversionRepository)
        : base(db)
    {
        _movementRepository = movementRepository;
        _currencyRepository = currencyRepository;
        _currencyConversionRepository = currencyConversionRepository;
    }

    public override async Task<DataResult<CurrencyConversion>> ExecuteAsync(CreateCurrencyConversionCommand command, CancellationToken cancellationToken)
    {
        var movement = await _movementRepository.GetByIdAsync(command.MovementId, cancellationToken);
        if (movement == null) throw new Exception("Movement not found");

        var newCurrencyConversion = new CurrencyConversion()
        {
            Movement = movement,
            Currency = command.CurrencyId.HasValue ? await _currencyRepository.GetByIdAsync(command.CurrencyId.Value, cancellationToken) : null,
            Amount = command.Amount,
        };

        await _currencyConversionRepository.AddAsync(newCurrencyConversion, cancellationToken);

        return DataResult<CurrencyConversion>.Success(newCurrencyConversion);
    }
}

public class CreateCurrencyConversionCommand : ICommand
{
    required public Guid MovementId { get; set; }
    public Guid? CurrencyId { get; set; }
    public Money Amount { get; set; }
}
