using FinanceApi.Application.Base.Handlers;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;
using MediatR;

namespace FinanceApi.Application.Commands.CurrencyConvertions;

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
        var currencyConversion = await currencyConversionRepository.GetById(command.Id);

        currencyConversion.Movement = await movementRepository.GetById(command.MovementId);

        currencyConversion.Currency = command.CurrencyId.HasValue ? await currencyRepository.GetById(command.CurrencyId.Value) : null;

        currencyConversion.Amount = command.Amount;

        await currencyConversionRepository.Update(currencyConversion);

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
