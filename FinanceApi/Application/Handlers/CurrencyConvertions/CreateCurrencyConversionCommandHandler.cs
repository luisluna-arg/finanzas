using FinanceApi.Application.Commands.CurrencyConversions;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.CurrencyConvertions;

public class CreateCurrencyConversionCommandHandler : BaseResponseHandler<CreateCurrencyConversionCommand, CurrencyConversion>
{
    private readonly IRepository<Movement, Guid> movementRepository;

    private readonly IRepository<Currency, Guid> currencyRepository;

    public CreateCurrencyConversionCommandHandler(
        FinanceDbContext db,
        IRepository<Movement, Guid> movementRepository,
        IRepository<Currency, Guid> currencyRepository)
        : base(db)
    {
        this.movementRepository = movementRepository;
        this.currencyRepository = currencyRepository;
    }

    public override async Task<CurrencyConversion> Handle(CreateCurrencyConversionCommand command, CancellationToken cancellationToken)
    {
        var newCurrencyConversion = new CurrencyConversion()
        {
            Movement = await movementRepository.GetById(command.MovementId),
            Currency = command.CurrencyId.HasValue ? await currencyRepository.GetById(command.CurrencyId.Value) : null,
            Amount = command.Amount,
        };

        DbContext.CurrencyConversion.Add(newCurrencyConversion);
        await DbContext.SaveChangesAsync();

        return await Task.FromResult(newCurrencyConversion);
    }
}
