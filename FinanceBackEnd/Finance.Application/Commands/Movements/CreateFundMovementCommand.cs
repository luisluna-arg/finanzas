using CQRSDispatch;
using Finance.Application.Base.Handlers;
using Finance.Application.Repositories;
using Finance.Domain.Models.AppModules;
using Finance.Domain.Models.Currencies;
using Finance.Domain.Models.Movements;
using Finance.Persistence;

namespace Finance.Application.Commands.Movements;

public class CreateFundMovementCommand : CreateMovementBaseCommand;

public class CreateFundMovementCommandHandler(
    FinanceDbContext db,
    IRepository<Movement, Guid> movementRepository,
    IRepository<Currency, Guid> currencyRepository,
    IAppModuleRepository appModuleRepository)
    : BaseCommandHandler<CreateFundMovementCommand, Movement>(db)
{
    private readonly IRepository<Movement, Guid> _movementRepository = movementRepository;
    private readonly IRepository<Currency, Guid> _currencyRepository = currencyRepository;
    private readonly IAppModuleRepository _appModuleRepository = appModuleRepository;

    public override async Task<DataResult<Movement>> ExecuteAsync(CreateFundMovementCommand command, CancellationToken cancellationToken)
    {
        AppModule? appModule = await _appModuleRepository.GetFundsAsync(cancellationToken);

        Currency? currency = command.CurrencyId.HasValue ? await _currencyRepository.GetByIdAsync(command.CurrencyId.Value, cancellationToken) : null;

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

        await _movementRepository.AddAsync(newMovement, cancellationToken);

        return DataResult<Movement>.Success(newMovement);
    }
}
