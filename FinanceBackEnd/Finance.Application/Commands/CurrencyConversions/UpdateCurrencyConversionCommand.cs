using Finance.Application.Base.Handlers;
using Finance.Domain;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using MediatR;

namespace Finance.Application.Commands.CurrencyConvertions;

public class UpdateCurrencyConversionCommandHandler : BaseResponseHandler<UpdateCurrencyConversionCommand, CurrencyConversion>
{
    private readonly IRepository<Currency, Guid> currencyRepository;
    private readonly IRepository<CurrencyConversion, Guid> currencyConversionRepository;
    private readonly IRepository<Movement, Guid> movementRepository;

    public UpdateCurrencyConversionCommandHandler(
        FinanceDbContext db,
        IRepository<Currency, Guid> currencyRepository,
        IRepository<CurrencyConversion, Guid> currencyConversionRepository,
        IRepository<Movement, Guid> movementRepository)
        : base(db)
    {
        this.currencyRepository = currencyRepository;
        this.currencyConversionRepository = currencyConversionRepository;
        this.movementRepository = movementRepository;
    }

    public override async Task<CurrencyConversion> Handle(UpdateCurrencyConversionCommand command, CancellationToken cancellationToken)
    {
        var currencyConversion = await currencyConversionRepository.GetByIdAsync(command.Id, cancellationToken);
        if (currencyConversion == null) throw new Exception("Currency Conversion not found");

        var movement = await movementRepository.GetByIdAsync(command.MovementId, cancellationToken);
        if (movement == null) throw new Exception("Movement not found");

        currencyConversion.Movement = movement;

        currencyConversion.Currency = command.CurrencyId.HasValue ? await currencyRepository.GetByIdAsync(command.CurrencyId.Value, cancellationToken) : null;

        currencyConversion.Amount = command.Amount;

        await currencyConversionRepository.UpdateAsync(currencyConversion, cancellationToken);

        return await Task.FromResult(currencyConversion);
    }
}

public class UpdateCurrencyConversionCommand : IRequest<CurrencyConversion>
{
    required public Guid Id { get; set; }

    required public Guid MovementId { get; set; }

    public Guid? CurrencyId { get; set; }

    public decimal Amount { get; set; }
}
