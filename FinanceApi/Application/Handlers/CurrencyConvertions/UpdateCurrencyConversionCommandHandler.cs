using FinanceApi.Application.Commands.CurrencyConversions;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.CurrencyConvertions;

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

        await DbContext.SaveChangesAsync();

        return await Task.FromResult(currencyConversion);
    }
}
