using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;
using MediatR;

namespace FinanceApi.Application.Commands.CurrencyConvertions;

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
        var newCurrencyConversion = new CurrencyConversion()
        {
            Movement = await movementRepository.GetById(command.MovementId),
            Currency = command.CurrencyId.HasValue ? await currencyRepository.GetById(command.CurrencyId.Value) : null,
            Amount = command.Amount,
        };

        await this.currencyConversionRepository.Add(newCurrencyConversion);

        return await Task.FromResult(newCurrencyConversion);
    }
}

public class CreateCurrencyConversionCommand : IRequest<CurrencyConversion>
{
    required public Guid MovementId { get; set; }

    public Guid? CurrencyId { get; set; }

    public decimal Amount { get; set; }
}
