using FinanceApi.Application.Commands.Movements;
using FinanceApi.Domain;
using FinanceApi.Domain.Models;
using FinanceApi.Infrastructure.Repositotories;

namespace FinanceApi.Application.Handlers.Movements;

public class CreateFundMovementCommandHandler : BaseResponseHandler<CreateFundMovementCommand, Movement>
{
    private readonly IRepository<Currency, Guid> _currencyRepository;
    private readonly IAppModuleRepository _appModuleRepository;

    public CreateFundMovementCommandHandler(
        FinanceDbContext db,
        IRepository<Currency, Guid> currencyRepository,
        IAppModuleRepository appModuleRepository)
        : base(db)
    {
        _currencyRepository = currencyRepository;
        _appModuleRepository = appModuleRepository;
    }

    public override async Task<Movement> Handle(CreateFundMovementCommand command, CancellationToken cancellationToken)
    {
        AppModule? appModule = await _appModuleRepository.GetFund();

        Currency? currency = command.CurrencyId.HasValue ? await _currencyRepository.GetById(command.CurrencyId.Value) : null;

        var newMovement = new Movement()
        {
            AppModule = appModule,
            Currency = currency,
            Amount = command.Amount,
            Concept1 = command.Concept1,
            Concept2 = command.Concept2,
            TimeStamp = command.TimeStamp,
            Total = command.Total,
        };

        DbContext.Movement.Add(newMovement);
        await DbContext.SaveChangesAsync();

        return await Task.FromResult(newMovement);
    }
}
