using Finance.Application.Base.Handlers;
using Finance.Domain.Models;
using Finance.Application.Repositories;
using Finance.Persistance;
using MediatR;

namespace Finance.Application.Commands.CurrencyConvertions;

public class CreateCurrencyConversionCommandHandler : BaseResponseHandler<CreateCurrencyConversionCommand, CurrencyConversion>
{
    private readonly IRepository<Movement, Guid> movementRepository;
    private readonly IRepository<Currency, Guid> currencyRepository;
    private readonly IRepository<CurrencyConversion, Guid> currencyConversionRepository;

    public CreateCurrencyConversionCommandHandler(
        FinanceDbContext db,
        IRepository<Movement, Guid> movementRepository,
        IRepository<Currency, Guid> currencyRepository,
        IRepository<CurrencyConversion, Guid> currencyConversionRepository)
        : base(db)
    {
        this.movementRepository = movementRepository;
        this.currencyRepository = currencyRepository;
        this.currencyConversionRepository = currencyConversionRepository;
    }

    public override async Task<CurrencyConversion> Handle(CreateCurrencyConversionCommand command, CancellationToken cancellationToken)
    {
        var movement = await movementRepository.GetByIdAsync(command.MovementId, cancellationToken);
        if (movement == null) throw new Exception("Movement not found");

        var newCurrencyConversion = new CurrencyConversion()
        {
            Movement = movement,
            Currency = command.CurrencyId.HasValue ? await currencyRepository.GetByIdAsync(command.CurrencyId.Value, cancellationToken) : null,
            Amount = command.Amount,
        };

        await this.currencyConversionRepository.AddAsync(newCurrencyConversion, cancellationToken);

        return await Task.FromResult(newCurrencyConversion);
    }
}

public class CreateCurrencyConversionCommand : IRequest<CurrencyConversion>
{
    required public Guid MovementId { get; set; }

    public Guid? CurrencyId { get; set; }

    public decimal Amount { get; set; }
}
